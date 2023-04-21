using Route256.Week6.Homework.PriceCalculator.Bll.Models;
using Route256.Week6.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week6.Homework.PriceCalculator.Dal.Entities;
using Route256.Week6.Homework.PriceCalculator.Dal.Models;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.Bll.Services;

public class CalculationService : ICalculationService
{
    public const decimal VolumeToPriceRatio = 3.27m;
    public const decimal WeightToPriceRatio = 1.34m;

    private readonly ICalculationRepository _calculationRepository;
    private readonly ICalculationsDlqKafkaRepository _calculationDlqKafkaRepository;

    private readonly IGoodsRepository _goodsRepository;

    public CalculationService(
        ICalculationRepository calculationRepository,
        IGoodsRepository goodsRepository,
        ICalculationsDlqKafkaRepository calculationDlqKafkaRepository)
    {
        _calculationRepository = calculationRepository;
        _goodsRepository = goodsRepository;
        _calculationDlqKafkaRepository = calculationDlqKafkaRepository;
    }

    public async Task<long> SaveCalculation(
        SaveCalculationModel data,
        CancellationToken cancellationToken)
    {
        var goods = data.Goods
            .Select(x => new GoodEntityV1
            {
                UserId = data.UserId,
                Height = x.Height,
                Weight = x.Weight,
                Length = x.Length,
                Width = x.Width
            })
            .ToArray();

        var calculation = new CalculationEntityV1
        {
            UserId = data.UserId,
            TotalVolume = data.TotalVolume,
            TotalWeight = data.TotalWeight,
            Price = data.Price,
            At = DateTimeOffset.UtcNow
        };

        using var transaction = _calculationRepository.CreateTransactionScope();
        var goodIds = await _goodsRepository.Add(goods, cancellationToken);

        calculation = calculation with { GoodIds = goodIds };
        var calculationIds = await _calculationRepository.Add(new[] { calculation }, cancellationToken);
        transaction.Complete();

        return calculationIds.Single();
    }

    public Task SaveInvalidRequestInDlq(long key, GoodModel goodModel, CancellationToken cancellationToken)
    {
        var entity = new DlqGoodEntityV1
        {
            Id = goodModel.Id,
            Height = goodModel.Properties.Height,
            Weight = goodModel.Properties.Weight,
            Length = goodModel.Properties.Length,
            Width = goodModel.Properties.Width,
        };

        return _calculationDlqKafkaRepository.Produce(key, entity, cancellationToken);
    }

    public decimal CalculatePriceByVolume(
        GoodPropertiesModel[] goods,
        out double volume)
    {
        volume = goods
            .Sum(x => x.Length * x.Width * x.Height);

        return (decimal)volume * VolumeToPriceRatio;
    }

    public decimal CalculatePriceByWeight(
        GoodPropertiesModel[] goods,
        out double weight)
    {
        weight = goods
            .Sum(x => x.Weight);

        return (decimal)weight * WeightToPriceRatio;
    }

    public async Task<QueryCalculationModel[]> QueryCalculations(
        QueryCalculationFilter query,
        CancellationToken token)
    {
        var result = await _calculationRepository.Query(new CalculationHistoryQueryModel(
                query.UserId,
                query.Limit,
                query.Offset),
            token);

        return result
            .Select(x => new QueryCalculationModel(
                x.Id,
                x.UserId,
                x.TotalVolume,
                x.TotalWeight,
                x.Price,
                x.GoodIds))
            .ToArray();
    }
}