using Moq;
using Route256.Week6.Homework.PriceCalculator.Bll.Services;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;

namespace Route256.Week6.Homework.PriceCalculator.UnitTests.Stubs;

public class CalculationServiceStub : CalculationService
{
    public Mock<ICalculationRepository> CalculationRepository { get; }
    public Mock<IGoodsRepository> GoodsRepository { get; }
    public Mock<ICalculationsDlqKafkaRepository> CalculationDlqRepository { get; }

    public CalculationServiceStub(
        Mock<ICalculationRepository> calculationRepository,
        Mock<IGoodsRepository> goodsRepository,
        Mock<ICalculationsDlqKafkaRepository> calculationDlqRepository)
        : base(
            calculationRepository.Object,
            goodsRepository.Object,
            calculationDlqRepository.Object)
    {
        CalculationRepository = calculationRepository;
        GoodsRepository = goodsRepository;
        CalculationDlqRepository = calculationDlqRepository;
    }

    public void VerifyNoOtherCalls()
    {
        CalculationRepository.VerifyNoOtherCalls();
        GoodsRepository.VerifyNoOtherCalls();
    }
}