namespace Unmatched.CatalogService.Domain.Registration;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.CatalogService.Domain.Mapping;
using Unmatched.CatalogService.Domain.Services;

public static class ServiceCollectionExtensions
{
    public static void RegisterDomainMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DomainMapper).Assembly);
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IKafkaProducer, KafkaProducer>();

        services.AddTransient<IHeroService, HeroService>();
        services.AddTransient<IMapService, MapService>();
        services.AddTransient<ISidekickService, SidekickService>();
        services.AddTransient<IPlayStyleService, PlayStyleService>();
    }
}
