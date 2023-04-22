using System.Threading.Channels;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Models;
using Route256.Week6.Homework.PriceCalculator.Bll.Commands;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.HostedServices;

public class AnomalousPricesDetectorHostedService : BackgroundService, IDisposable
{
    private readonly IDisposable? _topicsOptionsChangeListner;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AnomalousPricesDetectorHostedService> _logger;

    private readonly Channel<ConsumeResult<long, GoodPriceDto>> _goodPricesChanel =
    Channel.CreateUnbounded<ConsumeResult<long, GoodPriceDto>>(
        new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true,
        });

    private string? _pricesTopic;
    public string? PricesTopic
    {
        get => _pricesTopic;
        set
        {
            if (_pricesTopic != value)
            {
                _deliveryPricesConsumer.SwitchTopicTo(value);
                _pricesTopic = value;
            }
        }
    }

    private readonly InteractiveConsumer
        <long, GoodPriceDto, DeliveryPriceConsumerOptions> _deliveryPricesConsumer;

    public AnomalousPricesDetectorHostedService(
        IOptionsMonitor<Topics> topics,
        IOptions<DeliveryPriceConsumerOptions> priceConsumerOptions,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AnomalousPricesDetectorHostedService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        _deliveryPricesConsumer = new(priceConsumerOptions);

        _topicsOptionsChangeListner = topics.OnChange(options =>
        {
            PricesTopic = options.DeliveryPriceTopicName;
        });
        PricesTopic = topics.CurrentValue.DeliveryPriceTopicName;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var consumeTask = ConsumeGoodsPrices(stoppingToken);

        await foreach (var result in _goodPricesChanel.Reader.ReadAllAsync(stoppingToken))
        {
            var request = result.Message.Value;
            var command = new DetectPriceAnomalyCommand(
                new(
                    result.Message.Value.GoodId,
                    result.Message.Value.Price));

            var detected = await mediator.Send(command, stoppingToken);

            if (detected)
            {
                _logger.LogInformation("Anomaly was save into Database\n" +
                    $"{request}");
            }

            _deliveryPricesConsumer.Get().Commit(result);
        }

        await consumeTask;
        Dispose();
    }

    public override void Dispose()
    {
        _topicsOptionsChangeListner?.Dispose();
        _deliveryPricesConsumer.Dispose();
        _goodPricesChanel.Writer.Complete();

        base.Dispose();
    }

    private Task ConsumeGoodsPrices(CancellationToken token)
    {
        return Task.Factory.StartNew(async () =>
        {
            _deliveryPricesConsumer.Get().Subscribe(PricesTopic);
            var channelWriter = _goodPricesChanel.Writer;

            while (await channelWriter.WaitToWriteAsync())
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var result = _deliveryPricesConsumer.Get().Consume(token);

                        if (!channelWriter.TryWrite(result))
                        {
                            await channelWriter.WriteAsync(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Consume error: {ex.InnerException?.GetType()}\n" +
                            $"{ex.Message}",
                            ex.StackTrace);
                        break;
                    }
                }
            }
        }).Unwrap();
    }
}
