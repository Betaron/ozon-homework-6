using Microsoft.Extensions.DependencyInjection;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.HostedServices;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<DeliveryPriceCalculatorHostedService>();

        return services;
    }
}
