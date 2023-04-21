namespace Route256.Week6.Homework.PriceCalculator.Bll.Models;

public record SaveCalculationModel(
    long UserId,
    double TotalVolume,
    double TotalWeight,
    decimal Price,
    GoodPropertiesModel[] Goods);