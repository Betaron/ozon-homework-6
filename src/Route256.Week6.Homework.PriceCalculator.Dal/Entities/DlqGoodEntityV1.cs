namespace Route256.Week6.Homework.PriceCalculator.Dal.Entities;

public record DlqGoodEntityV1
{
    public long Id { get; set; }
    public double Height { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Weight { get; set; }
}
