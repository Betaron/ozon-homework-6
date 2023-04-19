namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations;

public record Topics
{
    public string GoodPropertiesTopicName { get; set; }
    public string DeliveryPriceTopicName { get; set; }
    public string DlqGoodPropertiesTopicName { get; set; }
}
