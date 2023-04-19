using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.Configurations;
using Route256.Week6.Homework.PriceCalculator.BackgroundServices.HostedServices;

namespace Route256.Week6.Homework.PriceCalculator.BackgroundServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostedServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoodsPropertiesConsumerOptions>(configuration.GetSection("CalculationBgServiceConfig:Options:GoodsPropertiesConsumerOptions"));
        services.Configure<DeliveryPriceProducerOptions>(configuration.GetSection("CalculationBgServiceConfig:Options:DeliveryPriceProducerOptions"));
        services.Configure<BadRequestsProducesesOptions>(configuration.GetSection("CalculationBgServiceConfig:Options:BadRequestsProducesesOptions"));
        services.Configure<Topics>(configuration.GetSection("CalculationBgServiceConfig:Topics"));

        services.AddHostedService<DeliveryPriceCalculatorHostedService>();

        return services;
    }
}
