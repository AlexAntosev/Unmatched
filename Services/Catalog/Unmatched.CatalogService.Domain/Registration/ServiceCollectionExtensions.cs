namespace Unmatched.CatalogService.Domain.Registration;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.CatalogService.Domain.Services;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IKafkaProducer, KafkaProducer>();

        services.AddTransient<IHeroService, HeroService>();
        services.AddTransient<IMapService, MapService>();
        services.AddTransient<IPlayStyleService, PlayStyleService>();
    }
}
