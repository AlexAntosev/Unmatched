namespace Unmatched.CatalogService.Api.Registration;

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.CatalogService.Api.Mapping;

public static class ServiceCollectionExtensions
{
    public static void RegisterApiMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(ApiMapper).Assembly);
    }
}
