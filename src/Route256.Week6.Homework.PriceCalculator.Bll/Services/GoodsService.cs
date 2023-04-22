using Route256.Week6.Homework.PriceCalculator.Bll.Models;
using Route256.Week6.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week6.Homework.PriceCalculator.Dal.Entities;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.Bll.Services;

internal class GoodsService : IGoodsService
{
    private readonly IGoodsDeliveryAnomalyRepository _goodsAnomalyRepository;

    public GoodsService(IGoodsDeliveryAnomalyRepository goodsAnomalyRepository)
    {
        _goodsAnomalyRepository = goodsAnomalyRepository;
    }

    public async Task<long[]> SaveAnomalousDelivery(
    GoodDeliveryPriceModel model,
        CancellationToken token)
    {
        var entity = new GoodDeliveryPriceEntityV1()
        {
            GoodId = model.GoodId,
            Price = model.Price
        };

        using var transaction = _goodsAnomalyRepository.CreateTransactionScope();
        var ids = await _goodsAnomalyRepository.Add(new[] { entity }, token);
        transaction.Complete();

        return ids;
    }
}
