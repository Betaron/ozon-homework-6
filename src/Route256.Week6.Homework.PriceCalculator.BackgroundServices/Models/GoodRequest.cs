namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Models;

internal record GoodRequest(
    long GoodId,
    double Height,
    double Length,
    double Width,
    double Weight);
