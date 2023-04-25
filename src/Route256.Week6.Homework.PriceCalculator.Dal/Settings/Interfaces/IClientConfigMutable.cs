using Confluent.Kafka;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Settings.Interfaces;

public interface IClientConfigMutable
{
    ClientConfig ToClientConfig();
}
