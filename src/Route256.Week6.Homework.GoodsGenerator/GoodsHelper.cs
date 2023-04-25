using Confluent.Kafka;
using Route256.Week6.Homework.GoodsGenerator.Models;

namespace Route256.Week6.Homework.GoodsGenerator;

internal class GoodsHelper
{
    private long _idCounter;

    public int From { get; set; }
    public int To { get; set; }

    public GoodsHelper(int from = 1, int to = 50)
    {
        _idCounter = 0;

        From = from;
        To = to;
    }

    public IEnumerable<GoodModel> GenerateGoods(int quantity)
    {
        var random = new Random();

        for (int i = 0; i < quantity; i++)
        {
            _idCounter++;

            yield return new GoodModel(
                _idCounter,
                random.Next(From, To),
                random.Next(From, To),
                random.Next(From, To),
                random.Next(From, To));
        }
    }

    public void GoodProduce(
        IProducer<long, GoodModel> producer,
        string topic,
        GoodModel good,
        CancellationToken token)
    {
        producer.ProduceAsync(
            topic,
            new Message<long, GoodModel>
            {
                Key = good.GoodId,
                Value = good
            },
            token);
    }
}
