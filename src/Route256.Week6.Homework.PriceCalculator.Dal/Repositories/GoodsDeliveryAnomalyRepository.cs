using Dapper;
using Microsoft.Extensions.Options;
using Route256.Week6.Homework.PriceCalculator.Dal.Entities;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Repositories;

internal class GoodsDeliveryAnomalyRepository : BaseRepository, IGoodsDeliveryAnomalyRepository
{
    public GoodsDeliveryAnomalyRepository(
        IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<long[]> Add(GoodDeliveryPriceEntityV1[] goodsDeliveryPrices, CancellationToken token)
    {
        const string sqlQuery = @"
insert into goods_delivery_anomalies (good_id, price) 
select good_id, price
  from UNNEST(@GoodsDeliveryPrices)
returning id;
";

        var sqlQueryParams = new
        {
            GoodsDeliveryPrices = goodsDeliveryPrices
        };

        await using var connection = await GetAndOpenConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                sqlQueryParams,
                cancellationToken: token));

        return ids
            .ToArray();
    }
}
