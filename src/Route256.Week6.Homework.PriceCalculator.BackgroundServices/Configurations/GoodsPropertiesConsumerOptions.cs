using Confluent.Kafka;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations;

public sealed class GoodsPropertiesConsumerOptions : IClientConfigMutable
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public AutoOffsetReset AutoOffsetReset { get; set; }
    public bool EnableAutoCommit { get; set; }
    public bool EnableAutoOffsetStore { get; set; }

    public ClientConfig ToClientConfig() =>
        new ConsumerConfig()
        {
            BootstrapServers = BootstrapServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset,
            EnableAutoOffsetStore = EnableAutoOffsetStore,
            EnableAutoCommit = EnableAutoCommit
        };
}
