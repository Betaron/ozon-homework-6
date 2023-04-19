using Confluent.Kafka;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations.Interfaces;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Utills;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;

internal class ClientBase
{
    protected static IConsumer<Ignore, TModel> BuildConsumer<TModel, TOptions>(TOptions options)
        where TModel : class
        where TOptions : IClientConfigMutable
    {
        return new ConsumerBuilder<Ignore, TModel>(options.ToClientConfig())
            .SetValueDeserializer(new JsonValueSerializer<TModel>())
            .Build();
    }

    protected IProducer<Ignore, TModel> BuildProducer<TModel, TOptions>(TOptions options)
        where TModel : class
        where TOptions : IClientConfigMutable
    {
        return new ProducerBuilder<Ignore, TModel>(options.ToClientConfig())
            .SetValueSerializer(new JsonValueSerializer<TModel>())
            .Build();
    }
}
