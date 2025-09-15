namespace Unmatched.MatchService.Domain.Registration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Unmatched.MatchService.Domain.Catalog;
using Unmatched.MatchService.Domain.Mapping;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.TitleHandlers;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IKafkaProducer, KafkaProducer>();

        services.AddSingleton<ICatalogHeroCache, CatalogHeroCache>();
        services.AddSingleton<ICatalogMapCache, CatalogMapCache>();
        services.AddSingleton<ICatalogSidekickCache, CatalogSidekickCache>();

        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IRatingService, RatingService>();
        services.AddTransient<ITournamentService, TournamentService>();

        services.AddTransient<IRatingCalculator, RatingCalculator>();
        services.AddTransient<IUnrankedRatingCalculator, UnrankedRatingCalculator>();
        services.AddTransient<IFirstTournamentRatingCalculator, FirstTournamentRatingCalculator>();

        services.AddTransient<IStreakTitleHandler, StreakTitleHandler>();
        services.AddTransient<IRusherTitleHandler, RusherTitleHandler>();
        services.AddTransient<IPunisherTitleHandler, PunisherTitleHandler>();
        services.AddTransient<IMatchHandlerFactory, MatchHandlerFactory>();

        services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
            {
                var baseUrl = configuration["Services:CatalogService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
    }

    public static void RegisterDomainMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DomainMapper).Assembly);
    }
}
