namespace Unmatched.Registration;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.DataInitialization;
using Unmatched.Entities;
using Unmatched.Services;
using Unmatched.Services.MatchHandlers;
using Unmatched.Services.RatingCalculators;

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
    }

    public static void RegisterMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IUnmatchedService, UnmatchedService>();
        services.AddTransient<IRatingCalculator, RatingCalculator>();
        services.AddTransient<IFirstTournamentRatingCalculator, FirstTournamentRatingCalculator>();
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IRatingService, RatingService>();
        services.AddTransient<IMatchHandlerFactory, MatchHandlerFactory>();
    }
}
