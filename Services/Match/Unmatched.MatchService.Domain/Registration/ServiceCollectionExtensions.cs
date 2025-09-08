namespace Unmatched.MatchService.Domain.Registration;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.TitleHandlers;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IKafkaProducer, KafkaProducer>();

        services.AddTransient<IRatingCalculator, RatingCalculator>();
        services.AddTransient<IUnrankedRatingCalculator, UnrankedRatingCalculator>();
        services.AddTransient<IFirstTournamentRatingCalculator, FirstTournamentRatingCalculator>();
        services.AddTransient<IStreakTitleHandler, StreakTitleHandler>();
        services.AddTransient<IRusherTitleHandler, RusherTitleHandler>();
        services.AddTransient<IPunisherTitleHandler, PunisherTitleHandler>();
        services.AddTransient<IMatchHandlerFactory, MatchHandlerFactory>();
    }
}
