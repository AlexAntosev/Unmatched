namespace Unmatched.MatchService.Domain.Registration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Player;
using Unmatched.MatchService.Domain.Mapping;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Titles.Rules;
using Unmatched.MatchService.Domain.Tournaments;
using Unmatched.MatchService.Domain.Validation;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IKafkaProducer, KafkaProducer>();

        services.AddSingleton<ICatalogHeroCache, CatalogHeroCache>();
        services.AddSingleton<ICatalogMapCache, CatalogMapCache>();
        services.AddSingleton<ICatalogSidekickCache, CatalogSidekickCache>();
        services.AddSingleton<ICatalogVillainCache, CatalogVillainCache>();
        services.AddSingleton<ICatalogMinionCache, CatalogMinionCache>();
        services.AddSingleton<IPlayerCache, PlayerCache>();

        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IRatingService, RatingService>();
        services.AddTransient<RatingTimeline>();
        services.AddTransient<ITournamentService, TournamentService>();
        services.AddTransient<ITournamentFormatGeneratorFactory, TournamentFormatGeneratorFactory>();
        services.AddTransient<Tournaments.TournamentAwardScheduler>();
        services.AddTransient<Tournaments.BountyChallengeResolver>();
        services.AddTransient<ITitleService, TitleService>();

        services.AddTransient<IRatingCalculatorFactory, RatingCalculatorFactory>();
        services.AddTransient<IGameModeValidatorFactory, GameModeValidatorFactory>();
        services.AddTransient<RankedMatchDataValidator>();

        services.AddTransient<TitleEvaluator>();
        services.AddTransient<TournamentTitleAwarder>();
        services.AddTransient<ITitleRule, FlawlessTitleRule>();
        services.AddTransient<ITitleRule, LastBreathTitleRule>();
        services.AddTransient<ITitleRule, GiantSlayerTitleRule>();
        services.AddTransient<ITitleRule, DeckMillerTitleRule>();
        services.AddTransient<ITitleRule, StreakTitleRule>();
        services.AddTransient<ITitleRule, SuffererTitleRule>();
        services.AddTransient<ITitleRule, GrandChampionTitleRule>();
        services.AddTransient<ITitleRule, KingslayerTitleRule>();
        services.AddTransient<ITitleRule, ExecutionerTitleRule>();
        services.AddTransient<ITitleRule, WallTitleRule>();
        services.AddTransient<ITitleRule, WorkhorseTitleRule>();
        services.AddTransient<IMatchHandler, MatchHandler>();

        services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
            {
                var baseUrl = configuration["Services:CatalogService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
        services.AddHttpClient<IPlayerClient, PlayerClient>(client =>
            {
                var baseUrl = configuration["Services:PlayerService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
    }

    public static void RegisterDomainMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(DomainMapper).Assembly);
    }
}
