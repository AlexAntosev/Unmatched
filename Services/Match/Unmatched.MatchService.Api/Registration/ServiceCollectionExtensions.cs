namespace Unmatched.MatchService.Api.Registration;

using Microsoft.Extensions.DependencyInjection;

using Unmatched.MatchService.Api.Mapping;

public static class ServiceCollectionExtensions
{
    public static void RegisterApiMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(ApiMapper).Assembly);
    }
}
