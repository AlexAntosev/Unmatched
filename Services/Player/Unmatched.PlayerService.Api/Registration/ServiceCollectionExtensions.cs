namespace Unmatched.PlayerService.Api.Registration;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.PlayerService.Api.Mapping;

public static class ServiceCollectionExtensions
{
    public static void RegisterApiMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(ApiMapper).Assembly);
    }
}
