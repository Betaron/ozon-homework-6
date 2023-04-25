using Confluent.Kafka;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Settings;

public class DeliveryPriceConsumerOptions : IClientConfigMutable
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
