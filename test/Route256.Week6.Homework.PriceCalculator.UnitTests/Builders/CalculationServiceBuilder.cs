using Moq;
using Route256.Week5.Workshop.PriceCalculator.UnitTests.Stubs;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;

namespace Route256.Week5.Workshop.PriceCalculator.UnitTests.Builders;

public class CalculationServiceBuilder
{
    public Mock<ICalculationRepository> CalculationRepository;
    public Mock<IGoodsRepository> GoodsRepository;
    
    public CalculationServiceBuilder()
    {
        CalculationRepository = new Mock<ICalculationRepository>();
        GoodsRepository = new Mock<IGoodsRepository>();
    }
    
    public CalculationServiceStub Build()
    {
        return new CalculationServiceStub(
            CalculationRepository,
            GoodsRepository);
    }
}