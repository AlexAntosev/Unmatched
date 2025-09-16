namespace Unmatched.StatisticsService.EntityFramework.Registration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Unmatched.StatisticsService.EntityFramework.Context;

public static class ServiceProviderExtensions
{
    public static void Migrate(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<UnmatchedDbContext>();
    
        dbContext.Database.Migrate();
    }
}
