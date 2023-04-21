using Route256.Week6.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week6.Homework.PriceCalculator.Bll.Services.Interfaces;

public interface ICalculationService
{
    Task<long> SaveCalculation(
        SaveCalculationModel saveCalculation,
        CancellationToken cancellationToken);

    Task SaveInvalidRequestInDlq(
        GoodModel goodModel,
        CancellationToken cancellationToken);

    decimal CalculatePriceByVolume(
        GoodPropertiesModel[] goods,
        out double volume);

    public decimal CalculatePriceByWeight(
        GoodPropertiesModel[] goods,
        out double weight);

    Task<QueryCalculationModel[]> QueryCalculations(
        QueryCalculationFilter query,
        CancellationToken token);
}