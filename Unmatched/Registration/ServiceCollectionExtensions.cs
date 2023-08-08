using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Unmatched.Services;

namespace Unmatched.Registration;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IUnmatchedService, UnmatchedService>();
        services.AddTransient<IRatingCalculator, RatingCalculator>();
    }
    
    public static void RegisterMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}