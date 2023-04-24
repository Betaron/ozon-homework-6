using Moq;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week6.Homework.PriceCalculator.UnitTests.Stubs;

namespace Route256.Week6.Homework.PriceCalculator.UnitTests.Builders;

public class CalculationServiceBuilder
{
    public Mock<ICalculationRepository> CalculationRepository;
    public Mock<IGoodsRepository> GoodsRepository;
    public Mock<ICalculationsDlqKafkaRepository> CalculationDlqRepository;

    public CalculationServiceBuilder()
    {
        CalculationRepository = new Mock<ICalculationRepository>();
        GoodsRepository = new Mock<IGoodsRepository>();
        CalculationDlqRepository = new Mock<ICalculationsDlqKafkaRepository>();
    }

    public CalculationServiceStub Build()
    {
        return new CalculationServiceStub(
            CalculationRepository,
            GoodsRepository,
            CalculationDlqRepository);
    }
}