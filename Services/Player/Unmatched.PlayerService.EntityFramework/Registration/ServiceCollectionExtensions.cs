namespace Unmatched.PlayerService.EntityFramework.Registration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Unmatched.PlayerService.Domain.Repositories;
using Unmatched.PlayerService.EntityFramework.Context;
using Unmatched.PlayerService.EntityFramework.Repositories;

public static class ServiceCollectionExtensions
{
    public static void RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<UnmatchedDbContext>(options => options.UseSqlServer(connectionString));
    }

    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();
    }
}