namespace Unmatched.Registration;

using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.Services;
using Unmatched.Services.Catalog;
using Unmatched.Services.MatchHandlers;
using Unmatched.Services.RatingCalculators;
using Unmatched.Services.Statistics;
using Unmatched.Services.TitleHandlers;

public static class ServiceCollectionExtensions
{
    public static void RegisterMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
    }

    public static void RegisterServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddTransient<IRatingCalculator, RatingCalculator>();
        services.AddTransient<IUnrankedRatingCalculator, UnrankedRatingCalculator>();
        services.AddTransient<IFirstTournamentRatingCalculator, FirstTournamentRatingCalculator>();
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IRatingService, RatingService>();
        services.AddTransient<IMatchHandlerFactory, MatchHandlerFactory>();
        services.AddTransient<IHeroStatisticsService, HeroStatisticsService>();
        services.AddTransient<IPlayerStatisticsService, PlayerStatisticsService>();
        services.AddTransient<IMapStatisticsService, MapStatisticsService>();
        services.AddTransient<IMinionStatisticsService, MinionStatisticsService>();
        services.AddTransient<ITitleService, TitleService>();
        services.AddTransient<IHeroService, HeroService>();
        services.AddTransient<IStreakTitleHandler, StreakTitleHandler>();
        services.AddTransient<ITournamentService, TournamentService>();
        services.AddTransient<IPlayerService, PlayerService>();
        services.AddTransient<IMapService, MapService>();
        services.AddTransient<IRusherTitleHandler, RusherTitleHandler>();
        services.AddTransient<IPunisherTitleHandler, PunisherTitleHandler>();
        services.AddTransient<IFavoriteStatisticsService, FavoriteStatisticsService>();
        services.AddTransient<IFavoriteService, FavoriteService>();
        services.AddTransient<IVillainStatisticsService, VillainStatisticsService>();
        //services.AddTransient<IPlayStyleService, PlayStyleService>();

        services.AddSingleton<ICatalogHeroCache, CatalogHeroCache>();

        services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
            {
                var baseUrl = configuration["Services:CatalogService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
    }
}
