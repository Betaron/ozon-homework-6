using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;

internal class InteractiveConsumer<TKey, TModel, TOptions> : ClientBase, IDisposable
    where TModel : class
    where TOptions : class, IClientConfigMutable
{
    private readonly IConsumer<TKey, TModel> _consumer;

    public InteractiveConsumer(IOptions<TOptions> options)
    {
        _consumer = BuildConsumer<TKey, TModel, TOptions>(options.Value);
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }

    public IConsumer<TKey, TModel> Get() => _consumer;

    public void SwitchTopicTo(string? topic)
    {
        _consumer.Unsubscribe();
        _consumer.Subscribe(topic);
    }
}
