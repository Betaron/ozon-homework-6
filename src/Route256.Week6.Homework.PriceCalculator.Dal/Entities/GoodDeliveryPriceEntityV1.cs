namespace Route256.Week6.Homework.PriceCalculator.Dal.Entities;

public record GoodDeliveryPriceEntityV1
{
    public long Id { get; set; }
    public long GoodId { get; set; }
    public decimal Price { get; set; }
}