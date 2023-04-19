using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;

internal class InteractiveConsumer<TModel, TOptions> : ClientBase, IDisposable
    where TModel : class
    where TOptions : class, IClientConfigMutable
{
    private IConsumer<Ignore, TModel> _consumer;

    private readonly IDisposable? _consumerOptionsChangeListner;

    /// <summary>
    /// Invoke when settings are changed
    /// </summary>
    public Action? OnChange { get; set; }

    public InteractiveConsumer(IOptions<TOptions> options)
    {
        _consumer = BuildConsumer<TModel, TOptions>(options.Value);
    }

    public void Dispose()
    {
        _consumerOptionsChangeListner?.Dispose();
        _consumer.Close();
        _consumer.Dispose();
    }

    public IConsumer<Ignore, TModel> Get() => _consumer;
}