namespace Unmatched.Initializer.Registration;

using Microsoft.Extensions.DependencyInjection;
using Unmatched.Initializer.Initializers;

public static class ServiceCollectionExtensions
{
    public static void RegisterInitializers(this IServiceCollection services)
    {
        services.AddTransient<IDataInitializer, DataInitializer>();
    }
}
