using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.Dal.Entities;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings;
using Route256.Week6.Homework.PriceCalculator.Dal.Utills;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Repositories;

public class CalculationsDlqKafkaRepository : ICalculationsDlqKafkaRepository, IDisposable
{
    private readonly IDisposable? _topicsOptionsChangeListner;
    private string _dlqTopic;

    private readonly BadRequestsProducerOptions _options;

    public CalculationsDlqKafkaRepository(
        IOptionsMonitor<Topics> topics,
        IOptions<BadRequestsProducerOptions> options)
    {
        _options = options.Value;

        _topicsOptionsChangeListner = topics.OnChange(options =>
        {
            _dlqTopic = options.GoodPropertiesDlqTopicName;
        });
        _dlqTopic = topics.CurrentValue.GoodPropertiesDlqTopicName;
    }

    public void Dispose()
    {
        _topicsOptionsChangeListner?.Dispose();
    }

    public async Task Produce(DlqGoodEntityV1 entity, CancellationToken token)
    {
        using var producer = new ProducerBuilder<long, DlqGoodEntityV1>(
            new ProducerConfig()
            {
                BootstrapServers = _options.BootstrapServers,
                Acks = _options.Acks
            })
            .SetValueSerializer(new JsonValueSerializer<DlqGoodEntityV1>())
            .Build();

        await producer.ProduceAsync(
            _dlqTopic,
            new()
            {
                Key = entity.Id,
                Value = entity
            },
            token);
    }
}
