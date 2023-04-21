using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Clients;

internal class InteractiveProducer<TKey, TModel, TOptions> : ClientBase, IDisposable
    where TModel : class
    where TOptions : class, IClientConfigMutable
{
    private readonly IProducer<TKey, TModel> _producer;

    public string? Topic { get; set; }
    public Headers? Headers { get; set; }

    public InteractiveProducer(
        IOptions<TOptions> options,
        string? topic = null,
        Headers? headers = null)
    {
        _producer = BuildProducer<TKey, TModel, TOptions>(options.Value);
        Topic = topic;
        Headers = headers;
    }

    public void Dispose()
    {
        _producer.Dispose();
    }

    public IProducer<TKey, TModel> Get() => _producer;

    /// <summary>
    /// Продьюсит сообщение с использованием внутренне определенного топика и заголовков
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task QuicProduce(TModel messageValue, CancellationToken token)
    {
        if (Topic is null)
        {
            throw new ArgumentNullException(nameof(Topic));
        }

        await _producer.ProduceAsync(
            Topic,
            new Message<TKey, TModel>()
            {
                Headers = this.Headers,
                Value = messageValue
            },
            token);
    }

    /// <summary>
    /// Продюсит сообщение копируя все свойства ConsumeResult
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task ProduceBasedConsumeResultAsync(ConsumeResult<TKey, TModel> sample, string? topic, CancellationToken token)
    {
        if (Topic is null)
        {
            throw new ArgumentNullException(nameof(Topic));
        }

        await _producer.ProduceAsync(
            topic,
            new()
            {
                Headers = sample.Message.Headers,
                Key = sample.Message.Key,
                Value = sample.Message.Value,
                Timestamp = sample.Message.Timestamp
            },
            token);
    }
}
