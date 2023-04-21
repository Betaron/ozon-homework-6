using System.Text.Json;
using Route256.Week6.Homework.PriceCalculator.Dal.Extensions;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Utills;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
            name.ToSnakeCase();
}
