using System.Collections.Generic;
using System.Linq;
using AutoBogus;
using Bogus;
using Route256.Week6.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week6.Homework.PriceCalculator.UnitTests.Fakers;

public static class GoodModelFaker
{
    private static readonly object Lock = new();

    private static readonly Faker<GoodPropertiesModel> Faker = new AutoFaker<GoodPropertiesModel>();

    public static IEnumerable<GoodPropertiesModel> Generate(int count = 1)
    {
        lock (Lock)
        {
            return Enumerable.Repeat(Faker.Generate(), count);
        }
    }
}