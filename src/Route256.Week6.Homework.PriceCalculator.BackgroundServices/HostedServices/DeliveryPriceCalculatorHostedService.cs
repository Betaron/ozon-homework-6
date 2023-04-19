using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Models;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.HostedServices;
public class DeliveryPriceCalculatorHostedService : BackgroundService, IDisposable
{
    private readonly IDisposable? _topicsOptionsChangeListner;

    private readonly Channel<ConsumeResult<Ignore, GoodPropertiesModel>> _goodPropertiesChanel =
        Channel.CreateUnbounded<ConsumeResult<Ignore, GoodPropertiesModel>>(
            new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true,
            });

    private readonly InteractiveConsumer
        <GoodPropertiesModel, GoodsPropertiesConsumerOptions> _goodPropertiesConsumer;

    private string _goodPropertiesTopic;
    private string GoodPropertiesTopic
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
        IOptions<GoodsPropertiesConsumerOptions> goodConsumerOptions)
    {
        _goodPropertiesConsumer = new(goodConsumerOptions);

        _topicsOptionsChangeListner = topics.OnChange(options =>
        {
            GoodPropertiesTopic = options.GoodPropertiesTopicName;
        });
        GoodPropertiesTopic = topics.CurrentValue.GoodPropertiesTopicName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var t = StartListenGoodsProperties(stoppingToken);

        await foreach (var res in _goodPropertiesChanel.Reader.ReadAllAsync(stoppingToken))
        {


            _goodPropertiesConsumer.Get().Commit();
        }
        await t;
        Dispose();
    }

    public override void Dispose()
    {
        _goodPropertiesConsumer.Dispose();
        _topicsOptionsChangeListner?.Dispose();

        _goodPropertiesChanel.Writer.Complete();
        base.Dispose();
    }

    private Task StartListenGoodsProperties(CancellationToken token)
    {
        return Task.Factory.StartNew(async () =>
        {
            _goodPropertiesConsumer.Get().Subscribe(GoodPropertiesTopic);
            var channelWriter = _goodPropertiesChanel.Writer;
            while (await channelWriter.WaitToWriteAsync())
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var result = _goodPropertiesConsumer.Get().Consume(token);
                        if (!channelWriter.TryWrite(result))
                        {
                            await channelWriter.WriteAsync(result);
                        }
                        _goodPropertiesConsumer.Get().StoreOffset(result);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }
        }).Unwrap();
    }
}
