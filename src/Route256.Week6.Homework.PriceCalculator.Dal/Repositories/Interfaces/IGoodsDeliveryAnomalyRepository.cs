using Route256.Week6.Homework.PriceCalculator.Dal.Entities;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;

public interface IGoodsDeliveryAnomalyRepository : IDbRepository
{
    Task<long[]> Add(
        GoodDeliveryPriceEntityV1[] goodsDeliveryPrices,
        CancellationToken token);
}
