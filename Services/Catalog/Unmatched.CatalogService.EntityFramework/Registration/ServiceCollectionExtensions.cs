namespace Unmatched.CatalogService.EntityFramework.Registration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Unmatched.CatalogService.EntityFramework.Context;
using Unmatched.CatalogService.EntityFramework.Repositories;

public static class ServiceCollectionExtensions
{
    public static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<UnmatchedDbContext>(options => options.UseSqlServer(connectionString));
    }

    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IHeroRepository, HeroRepository>();
        services.AddTransient<ISidekickRepository, SidekickRepository>();
        services.AddTransient<IMapRepository, MapRepository>();
        services.AddTransient<IMinionRepository, MinionRepository>();
        services.AddTransient<IVillainRepository, VillainRepository>();
        services.AddTransient<IPlayStyleRepository, PlayStyleRepository>();
    }
}