using Route256.Week6.Homework.PriceCalculator.Dal.Entities;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;

public interface ICalculationsDlqKafkaRepository
{
    public Task Produce(DlqGoodEntityV1 entity, CancellationToken token);
}
