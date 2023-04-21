using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Route256.Week6.Homework.PriceCalculator.Dal.Infrastructure;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories;
using Route256.Week6.Homework.PriceCalculator.Dal.Repositories.Interfaces;
using Route256.Week6.Homework.PriceCalculator.Dal.Settings;

namespace Route256.Week6.Homework.PriceCalculator.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<ICalculationRepository, CalculationRepository>();
        services.AddScoped<IGoodsRepository, GoodsRepository>();

        services.AddSingleton<ICalculationsDlqKafkaRepository, CalculationsDlqKafkaRepository>();

        return services;
    }

    public static IServiceCollection AddKafkaOptions(
       this IServiceCollection services,
       IConfigurationRoot config)
    {
        services.Configure<BadRequestsProducerOptions>(
            config.GetSection("KafkaOptions:ClientsOptions:BadRequestsProducesesOptions"));
        services.Configure<DeliveryPriceProducerOptions>(
            config.GetSection("KafkaOptions:ClientsOptions:DeliveryPriceProducerOptions"));
        services.Configure<GoodsPropertiesConsumerOptions>(
            config.GetSection("KafkaOptions:ClientsOptions:GoodsPropertiesConsumerOptions"));
        services.Configure<Topics>(
            config.GetSection("KafkaOptions:Topics"));

        return services;
    }

    public static IServiceCollection AddDalInfrastructure(
    this IServiceCollection services,
    IConfigurationRoot config)
    {
        //read config
        services.Configure<DalOptions>(config.GetSection(nameof(DalOptions)));

        //configure postrges types
        Postgres.MapCompositeTypes();

        //add migrations
        Postgres.AddMigrations(services);

        return services;

    }
}