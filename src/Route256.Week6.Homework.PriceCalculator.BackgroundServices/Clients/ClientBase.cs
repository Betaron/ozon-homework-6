using Confluent.Kafka;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings.Interfaces;
using Route256.Week6.Homework.PriceCalculator.Dal.Utills;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;

internal class ClientBase
{
    protected static IConsumer<TKey, TModel> BuildConsumer<TKey, TModel, TOptions>(TOptions options)
        where TModel : class
        where TOptions : IClientConfigMutable
    {
        return new ConsumerBuilder<TKey, TModel>(options.ToClientConfig())
            .SetValueDeserializer(new JsonValueSerializer<TModel>())
            .Build();
    }

    protected IProducer<TKey, TModel> BuildProducer<TKey, TModel, TOptions>(TOptions options)
        where TModel : class
        where TOptions : IClientConfigMutable
    {
        return new ProducerBuilder<TKey, TModel>(options.ToClientConfig())
            .SetValueSerializer(new JsonValueSerializer<TModel>())
            .Build();
    }
}
