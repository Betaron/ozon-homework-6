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

        _deliveryPricesConsumer.Get().Subscribe(PricesTopic);

        await Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _deliveryPricesConsumer.Get().Consume(stoppingToken);

                    await DetectAnomaly(result, mediator, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Consume error: {ex.InnerException?.GetType()}\n" +
                        $"{ex.Message}" +
                        ex.StackTrace);
                    break;
                }
            }
        });

        Dispose();
    }

    public override void Dispose()
    {
        _topicsOptionsChangeListner?.Dispose();
        _deliveryPricesConsumer.Dispose();

        base.Dispose();
    }

    private async Task DetectAnomaly(
        ConsumeResult<long, GoodPriceDto> result,
        IMediator mediator,
        CancellationToken token)
    {
        var request = result.Message.Value;
        var command = new DetectPriceAnomalyCommand(
            new(result.Message.Value.GoodId,
                result.Message.Value.Price));

        var detected = await mediator.Send(command, token);

        if (detected)
        {
            _logger.LogInformation("Anomaly was save into Database\n" +
                $"{request}");
        }

        _deliveryPricesConsumer.Get().Commit(result);
    }
}
