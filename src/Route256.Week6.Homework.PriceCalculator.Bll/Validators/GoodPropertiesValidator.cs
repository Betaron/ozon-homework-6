using FluentValidation;
using Route256.Week6.Homework.PriceCalculator.Bll.Models;

namespace Route256.Week6.Homework.PriceCalculator.Bll.Validators;

internal sealed class GoodValidator : AbstractValidator<GoodModel>
{
    public GoodValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Properties.Width)
            .GreaterThan(0);

        RuleFor(x => x.Properties.Height)
            .GreaterThan(0);

        RuleFor(x => x.Properties.Length)
            .GreaterThan(0);

        RuleFor(x => x.Properties.Weight)
            .GreaterThan(0);
    }
}
