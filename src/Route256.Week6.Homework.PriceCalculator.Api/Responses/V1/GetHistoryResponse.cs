namespace Route256.Week6.Homework.PriceCalculator.Api.Responses.V1;

public record GetHistoryResponse(
    GetHistoryResponse.CargoResponse Cargo,
    decimal Price)
{
    public record CargoResponse(
        double Volume,
        double Weight,
        long[] GoodIds);
}