namespace Unmatched.StatisticsService.Domain.Registration;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.StatisticsService.Domain.Initialize;

public static class ServiceProviderExtensions
{
    public static async Task InitializeAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var initializer = scope.ServiceProvider
            .GetRequiredService<IStatisticsInitializer>();

        await initializer.InitializeAsync();
    }
}
