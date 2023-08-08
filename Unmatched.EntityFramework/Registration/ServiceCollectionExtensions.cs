using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.EntityFramework.Context;
using Unmatched.EntityFramework.Repositories;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Registration;

public static class ServiceCollectionExtensions
{
    public static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<UnmatchedDbContext>(options => options.UseSqlServer(connectionString));
    }
        
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IPlayerRepository, PlayerRepository>();
        services.AddTransient<IHeroRepository, HeroRepository>();
        services.AddTransient<ILeagueRepository, LeagueRepository>();
        services.AddTransient<ISidekickRepository, SidekickRepository>();
        services.AddTransient<IMapRepository, MapRepository>();
        services.AddTransient<IDuelMatchRepository, DuelMatchRepository>();
        services.AddTransient<IOpponentRepository, OpponentRepository>();
    }
}