namespace Unmatched.StatisticsService.EntityFramework.Registration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.EntityFramework.Context;
using Unmatched.StatisticsService.EntityFramework.Mapping;
using Unmatched.StatisticsService.EntityFramework.Repositories;

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

    public static void RegisterEntityFrameworkMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(EfMapper).Assembly);
    }
}