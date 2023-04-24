using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Confluent.Kafka;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Models;
using Route256.Week6.Homework.PriceCalculator.Bll.Commands;
using Route256.Week6.Homework.PriceCalculator.Bll.Models;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.HostedServices;
public class DeliveryPriceCalculatorHostedService : BackgroundService, IDisposable
{
    private readonly IDisposable? _topicsOptionsChangeListner;

    private readonly Channel<ConsumeResult<long, GoodRequest>> _goodPropertiesChanel =
        Channel.CreateUnbounded<ConsumeResult<long, GoodRequest>>(
            new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true,
            });

    private readonly InteractiveConsumer
        <long, GoodRequest, GoodsPropertiesConsumerOptions> _goodPropertiesConsumer;
    private readonly InteractiveProducer
        <long, GoodPriceResponse, DeliveryPriceProducerOptions> _deliveryPriceProducer;

    private IProducer<byte[], byte[]> _rawGoodPropertiesDlqProducer;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DeliveryPriceCalculatorHostedService> _logger;

    private string _dlqTopic;
    private string? _goodPropertiesTopic;
    private string? GoodPropertiesTopic
    {
        get => _goodPropertiesTopic;
        set
        {
            if (_goodPropertiesTopic != value)
            {
                _goodPropertiesConsumer.SwitchTopicTo(value);
                _goodPropertiesTopic = value;
            }
        }
    }

    public DeliveryPriceCalculatorHostedService(
        IOptionsMonitor<Topics> topics,
        IOptions<GoodsPropertiesConsumerOptions> goodConsumerOptions,
        IOptions<BadRequestsProducerOptions> dlqProducerOptions,
        IOptions<DeliveryPriceProducerOptions> priceProducerOptions,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DeliveryPriceCalculatorHostedService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        _goodPropertiesConsumer = new(goodConsumerOptions);
        _deliveryPriceProducer = new(priceProducerOptions);

        _rawGoodPropertiesDlqProducer = new ProducerBuilder<byte[], byte[]>(
            dlqProducerOptions.Value.ToClientConfig()).Build();

        _topicsOptionsChangeListner = topics.OnChange(options =>
        {
            GoodPropertiesTopic = options.GoodPropertiesTopicName;
            _deliveryPriceProducer.Topic = options.DeliveryPriceTopicName;
            _dlqTopic = options.GoodPropertiesDlqTopicName;
        });
        GoodPropertiesTopic = topics.CurrentValue.GoodPropertiesTopicName;
        _deliveryPriceProducer.Topic = topics.CurrentValue.DeliveryPriceTopicName;
        _dlqTopic = topics.CurrentValue.GoodPropertiesDlqTopicName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var consumeTask = ConsumeGoodsProperties(stoppingToken);

        await foreach (var result in _goodPropertiesChanel.Reader.ReadAllAsync(stoppingToken))
        {
            var request = result.Message.Value;
            var command = new CalculateDeliveryPriceWithDlqCommand(
                result.Message.Key,
                new GoodModel(
                    request.GoodId,
                    new GoodPropertiesModel(
                        request.Height,
                        request.Length,
                        request.Width,
                        request.Width)));
            try
            {
                var price = await mediator.Send(command, stoppingToken);

                await _deliveryPriceProducer.QuicProduce(
                    new GoodPriceResponse(
                        price.GoodId,
                        price.Price),
                    stoppingToken);

                _logger.LogInformation("Calculate and produce success.");
            }
            catch (ValidationException validationEx)
            {
                var sb = new StringBuilder();
                foreach (var item in validationEx.Errors)
                {
                    sb.AppendLine(item.ErrorMessage);
                }
                _logger.LogWarning("Consume invalid data.\n" +
                    "Invalid message was produced in dlq." +
                    "ValidationErrors:\n" +
                    sb);
            }
            catch (Exception ProduceEx)
                when (ProduceEx is ProduceException<long, GoodPriceResponse> or ArgumentException)
            {
                _logger.LogError($"Produce error: {ProduceEx.InnerException?.GetType()}\n" +
                            $"{ProduceEx.Message}" +
                            ProduceEx.StackTrace);
                continue;
            }

            _goodPropertiesConsumer.Get().Commit(result);
        }

        await consumeTask;
        Dispose();
    }

    public override void Dispose()
    {
        _goodPropertiesConsumer.Dispose();
        _deliveryPriceProducer.Dispose();
        _topicsOptionsChangeListner?.Dispose();

        _goodPropertiesChanel.Writer.Complete();
        base.Dispose();
    }

    private Task ConsumeGoodsProperties(CancellationToken token)
    {
        return Task.Factory.StartNew(async () =>
        {
            _goodPropertiesConsumer.Get().Subscribe(GoodPropertiesTopic);
            var channelWriter = _goodPropertiesChanel.Writer;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = _goodPropertiesConsumer.Get().Consume(token);

                    if (!channelWriter.TryWrite(result))
                    {
                        await channelWriter.WriteAsync(result);
                    }
                }
                catch (ConsumeException ConsumeEx)
                    when (ConsumeEx.InnerException is JsonException or ArgumentException)
                {
                    var result = ConsumeEx.ConsumerRecord;

                    if (await ProduceCorruptedIntoDlq(result, token))
                    {
                        _logger.LogWarning($"Consume falure: {ConsumeEx.InnerException?.GetType()}\n" +
                        $"{ConsumeEx.InnerException?.Message}\n" +
                        "Corrupted message was produced in dlq.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Consume error: {ex.InnerException?.GetType()}\n" +
                        $"{ex.Message}\n" +
                        ex.StackTrace);
                    break;
                }
            }
        }).Unwrap();
    }

    private async Task<bool> ProduceCorruptedIntoDlq(
        ConsumeResult<byte[], byte[]> result,
        CancellationToken token)
    {
        try
        {
            await _rawGoodPropertiesDlqProducer.ProduceAsync(
                _dlqTopic,
                new()
                {
                    Key = result.Message.Key,
                    Value = result.Message.Value
                },
                token);

            _goodPropertiesConsumer.Get().Commit(
                new List<TopicPartitionOffset>()
                    { new(result.TopicPartition, result.Offset + 1) });
        }
        catch (Exception ProduceEx)
            when (ProduceEx is ProduceException<byte[], byte[]> or ArgumentException)
        {
            _logger.LogError($"Produce error: {ProduceEx.InnerException?.GetType()}\n" +
                        $"\t{ProduceEx.Message}\n" +
                        ProduceEx.StackTrace);

            return false;
        }
        return true;
    }
}
