namespace Route256.Week6.Homework.PriceCalculator.Bll.Models;

public record QueryCalculationFilter(
    long UserId,
    int Limit,
    int Offset);