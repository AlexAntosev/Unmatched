namespace Unmatched.StatisticsService.Api.Registration;

using Unmatched.StatisticsService.Api.Mapping;

public static class ServiceCollectionExtensions
{
    public static void RegisterApiMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(ApiMapper).Assembly);
    }
}
