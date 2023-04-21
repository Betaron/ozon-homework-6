namespace Route256.Week6.Homework.PriceCalculator.Dal.Settings;

public record DalOptions
{
    public string ConnectionString { get; init; } = string.Empty;
}