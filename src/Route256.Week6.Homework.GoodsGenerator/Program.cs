using Confluent.Kafka;
using Route256.Week6.Homework.GoodsGenerator;
using Route256.Week6.Homework.GoodsGenerator.Models;
using Route256.Week6.Homework.GoodsGenerator.Utills;

internal class Program
{
    public static void Main(string[] args)
    {
        var quantity = 0;
        do
        {
            Console.Write("Quantity to be generated: ");
            int.TryParse(Console.ReadLine(), out quantity);
        } while (quantity <= 0);


        var answer = string.Empty;
        do
        {
            Console.WriteLine("Are you sure? (y/n): ");
            answer = Console.ReadLine();
        } while (answer != "y" && answer != "n");

        if (answer == "n")
        {
            return;
        }

        var helper = new GoodsHelper();
        var goodGenerator = helper.GenerateGoods(quantity);

        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

        using var producer = new ProducerBuilder<long, GoodModel>(
                new ProducerConfig
                {
                    BootstrapServers = "localhost:9092",
                    Acks = Acks.All
                })
            .SetValueSerializer(new JsonValueSerializer<GoodModel>())
            .Build();

        foreach (var good in goodGenerator)
        {


            helper.GoodProduce(
                producer,
                "good_price_calc_requests",
                good,
                cts.Token);

        }

        producer.Flush();
    }
}