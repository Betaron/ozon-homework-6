using System.Text.Json;
using Route256.Week6.Homework.GoodsGenerator.Extensions;

namespace Route256.Week6.Homework.GoodsGenerator.Utills;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
            name.ToSnakeCase();
}
