namespace Unmatched.Registration;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.Data.Entities;
using Unmatched.DataInitialization;
using Unmatched.DataInitialization.Matches;
using Unmatched.Services;
using Unmatched.Services.MatchHandlers;
using Unmatched.Services.RatingCalculators;
using Unmatched.Services.Statistics;
using Unmatched.Services.TitleHandlers;

public static class ServiceCollectionExtensions
{
    public static void RegisterDataInitializers(this IServiceCollection services)
    {
        services.AddTransient<IDataInitializer<Hero>, HeroesDataInitializer>();
        services.AddTransient<IDataInitializer<Map>, MapsDataInitializer>();
        services.AddTransient<IDataInitializer<Player>, PlayersDataInitializer>();
        services.AddTransient<IDataInitializer<Sidekick>, SidekicksDataInitializer>();
        services.AddTransient<IDataInitializer<Tournament>, TournamentsDataInitializer>();
        services.AddTransient<IUnrankedMatchDataInitializer, UnrankedMatchDataInitializer>();
        services.AddTransient<IFirstTournamentMatchesDataInitializer, FirstTournamentMatchesDataInitializer>();
        services.AddTransient<IRankedMatchDataInitializer, RankedMatchDataInitializer>();
        services.AddTransient<IHoudiniVsGenieDadaInitializer, HoudiniVsGenieDadaInitializer>();
        services.AddTransient<ITalesOfAmazeDataInitializer, TalesOfAmazeDataInitializer>();
    }

    public static void RegisterMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    public static void RegisterServices(this IServiceCollection services)
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
        services.AddTransient<ITitleService, TitleService>();
        services.AddTransient<IHeroService, HeroService>();
        services.AddTransient<IStreakTitleHandler, StreakTitleHandler>();
        services.AddTransient<ITournamentService, TournamentService>();
        services.AddTransient<IPlayerService, PlayerService>();
        services.AddTransient<IMapService, MapService>();
        services.AddTransient<IRusherTitleHandler, RusherTitleHandler>();
    }
}
