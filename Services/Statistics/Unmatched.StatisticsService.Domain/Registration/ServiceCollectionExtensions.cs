namespace Unmatched.StatisticsService.Domain.Registration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Kafka;
using Unmatched.StatisticsService.Domain.Initialize;
using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Mapping;
using Unmatched.StatisticsService.Domain.Services;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public static class ServiceCollectionExtensions
{
    public static void RegisterDomainMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DomainMapper).Assembly);
    }

    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IHeroStatisticsService, HeroStatisticsService>();
        services.AddTransient<IMapStatisticsService, MapStatisticsService>();

        services.AddSingleton<ICatalogHeroCache, CatalogHeroCache>();
        services.AddSingleton<ICatalogMapCache, CatalogMapCache>();

        services.AddSingleton<IHeroPlaceAdjuster, HeroPlaceAdjuster>();

        services.AddTransient<IStatisticsInitializer, StatisticsInitializer>();
        services.AddTransient<IStatsCoordinatorProvider, StatsCoordinatorProvider>();

        services.AddTransient<IStatsCoordinator, HeroStatsCoordinator>();
        services.AddTransient<IStatsCoordinator, MapStatsCoordinator>();

        services.AddTransient<IMatchCreatedHandler, MatchCreatedHeroHandler>();
        services.AddTransient<IMatchCreatedHandler, MatchCreatedMapHandler>();

        services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
            {
                var baseUrl = configuration["Services:CatalogService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });

        services.AddHttpClient<IMatchClient, MatchClient>(client =>
            {
                var baseUrl = configuration["Services:MatchService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
    }

    public static void RegisterBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<MatchCreatedConsumer>();
    }
}