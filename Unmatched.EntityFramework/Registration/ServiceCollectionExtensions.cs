namespace Unmatched.EntityFramework.Registration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;
using Unmatched.EntityFramework.Repositories;

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
        services.AddTransient<IPlayerRepository, PlayerRepository>();
        services.AddTransient<IHeroRepository, HeroRepository>();
        services.AddTransient<ITournamentRepository, TournamentRepository>();
        services.AddTransient<ISidekickRepository, SidekickRepository>();
        services.AddTransient<IMapRepository, MapRepository>();
        services.AddTransient<IMatchRepository, MatchRepository>();
        services.AddTransient<IMinionRepository, MinionRepository>();
        services.AddTransient<IFighterRepository, FighterRepository>();
        services.AddTransient<IRatingRepository, RatingRepository>();
        services.AddTransient<ITitleRepository, TitleRepository>();
        services.AddTransient<IFavoritesRepository, FavoritesRepository>();
        services.AddTransient<IVillainRepository, VillainRepository>();
    }
}