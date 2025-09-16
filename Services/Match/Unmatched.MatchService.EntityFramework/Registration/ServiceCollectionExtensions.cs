namespace Unmatched.MatchService.EntityFramework.Registration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

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