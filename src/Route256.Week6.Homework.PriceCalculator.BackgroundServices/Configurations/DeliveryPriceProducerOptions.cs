using Confluent.Kafka;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations;

public class DeliveryPriceProducerOptions
{
    public string BootstrapServers { get; set; }
    public Acks Acks { get; set; }

    public ClientConfig ToClientConfig() =>
        new ConsumerConfig()
        {
            BootstrapServers = BootstrapServers,
            Acks = Acks
        };
}
