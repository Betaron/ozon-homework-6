using Moq;
using Route256.Week5.Workshop.PriceCalculator.UnitTests.Stubs;
using Route256.Week6.Homework.PriceCalculator.Bll.Services.Interfaces;

namespace Route256.Week5.Workshop.PriceCalculator.UnitTests.Builders;

public class CalculateDeliveryPriceHandlerBuilder
{
    public Mock<ICalculationService> CalculationService;
    
    public CalculateDeliveryPriceHandlerBuilder()
    {
        CalculationService = new Mock<ICalculationService>();
    }
    
    public CalculateDeliveryPriceCommandHandlerStub Build()
    {
        return new CalculateDeliveryPriceCommandHandlerStub(
            CalculationService);
    }
}