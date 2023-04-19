using Confluent.Kafka;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations.Interfaces;

internal interface IClientConfigMutable
{
    ClientConfig ToClientConfig();
}
