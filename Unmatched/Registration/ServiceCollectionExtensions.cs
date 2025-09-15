namespace Unmatched.Registration;

using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Unmatched.HttpClients;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services;
using Unmatched.Services.Contracts;
using Unmatched.Services.Statistics;

public static class ServiceCollectionExtensions
{
    public static void RegisterMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
    }

    public static void RegisterServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddTransient<IHeroService, HeroService>();
        services.AddTransient<IMapService, MapService>();
        services.AddTransient<IPlayStyleService, PlayStyleService>();
        services.AddTransient<IPlayerService, PlayerService>();
        services.AddTransient<IFavoriteService, FavoriteService>();
        services.AddTransient<IMatchService, MatchService>();
        services.AddTransient<IRatingService, RatingService>();
        services.AddTransient<ITournamentService, TournamentService>();


        services.AddTransient<IHeroStatisticsService, HeroStatisticsService>();

        // services.AddTransient<ITitleService, TitleService>();


        services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
            {
                var baseUrl = configuration["Services:CatalogService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
        services.AddHttpClient<IMatchClient, MatchClient>(client =>
            {
                var baseUrl = configuration["Services:MatchService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
        services.AddHttpClient<IPlayerClient, PlayerClient>(client =>
            {
                var baseUrl = configuration["Services:PlayerService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
        services.AddHttpClient<IStatisticsClient, StatisticsClient>(client =>
            {
                var baseUrl = configuration["Services:StatisticsService:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
            });
    }
}
