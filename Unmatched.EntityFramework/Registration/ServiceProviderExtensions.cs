namespace Unmatched.EntityFramework.Registration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.EntityFramework.Context;

public static class ServiceProviderExtensions
{
    public static void Migrate(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<UnmatchedDbContext>();
    
        dbContext.Database.EnsureCreated();
        dbContext.Database.Migrate();
    }
}
