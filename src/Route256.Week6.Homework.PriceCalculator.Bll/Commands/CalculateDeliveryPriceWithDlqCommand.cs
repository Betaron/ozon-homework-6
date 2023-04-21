using FluentValidation;
using MediatR;
using Route256.Week6.Homework.PriceCalculator.Bll.Models;
using Route256.Week6.Homework.PriceCalculator.Bll.Services.Interfaces;
using Route256.Week6.Homework.PriceCalculator.Bll.Validators;

namespace Route256.Week6.Homework.PriceCalculator.Bll.Commands;

public record CalculateDeliveryPriceWithDlqCommand(
    GoodModel Good)
    : IRequest<GoodDeliveryPriceModel>;

public class CalculateDeliveryPriceWithDlqCommandHandler
    : IRequestHandler<CalculateDeliveryPriceWithDlqCommand, GoodDeliveryPriceModel>
{
    private readonly ICalculationService _calculationService;
    private readonly GoodValidator _propertiesValidator = new();

    public CalculateDeliveryPriceWithDlqCommandHandler(
        ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    public async Task<GoodDeliveryPriceModel> Handle(
        CalculateDeliveryPriceWithDlqCommand request,
        CancellationToken cancellationToken)
    {
        var validateResult = _propertiesValidator.Validate(request.Good);
        if (!validateResult.IsValid)
        {
            await _calculationService.SaveInvalidRequestInDlq(request.Good, cancellationToken);
            throw new ValidationException(validateResult.Errors);
        }

        var volumePrice = _calculationService.CalculatePriceByVolume(new[] { request.Good.Properties }, out _);
        var weightPrice = _calculationService.CalculatePriceByWeight(new[] { request.Good.Properties }, out _);
        var resultPrice = Math.Max(volumePrice, weightPrice);

        return new GoodDeliveryPriceModel(
            request.Good.Id,
            resultPrice);
    }
}