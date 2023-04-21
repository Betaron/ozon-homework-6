namespace Route256.Week6.Homework.PriceCalculator.Api.Requests.V1;

public record GetHistoryRequest(
    long UserId,
    int Take,
    int Skip);