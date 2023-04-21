using Confluent.Kafka;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Settings;

public class DeliveryPriceProducerOptions : IClientConfigMutable
{
    public string BootstrapServers { get; set; }
    public Acks Acks { get; set; }

    public ClientConfig ToClientConfig() =>
        new ProducerConfig()
        {
            BootstrapServers = BootstrapServers,
            Acks = Acks
        };
}
