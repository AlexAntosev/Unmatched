namespace Unmatched.PlayerService.Domain.Registration;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.PlayerService.Domain.Services;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IFavoriteService, FavoriteService>();
        services.AddTransient<IPlayerService, PlayerService>();
    }
}
