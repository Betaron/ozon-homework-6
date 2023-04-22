using MediatR;
using Route256.Week6.Homework.PriceCalculator.Bll.Models;
using Route256.Week6.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.Bll.Commands;

public record DetectPriceAnomalyCommand(
        GoodDeliveryPriceModel GoodDeliveryPrice)
    : IRequest<bool>;

public class DetectPriceAnomalyCommandHandler
    : IRequestHandler<DetectPriceAnomalyCommand, bool>
{
    private readonly IGoodsService _goodsService;

    public DetectPriceAnomalyCommandHandler(
        IGoodsService goodsService)
    {
        _goodsService = goodsService;
    }

    public async Task<bool> Handle(
        DetectPriceAnomalyCommand request,
        CancellationToken cancellationToken)
    {
        if (request.GoodDeliveryPrice.Price < 10000m)
        {
            return false;
        }

        var ids = await _goodsService.SaveAnomalousDelivery(
            request.GoodDeliveryPrice,
            cancellationToken);

        return ids.Any();
    }
}