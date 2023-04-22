using Route256.Week6.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week6.Homework.PriceCalculator.Bll.Services.Interfaces;

public interface IGoodsService
{
    public Task<long[]> SaveAnomalousDelivery(
        GoodDeliveryPriceModel model,
        CancellationToken token);
}
