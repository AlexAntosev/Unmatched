namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Unmatched.MatchService.Api.Mapping;
using Unmatched.MatchService.Domain.Mapping;

public class MapperTest
{
    [Fact]
    public void CheckMappings()
    {
        var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainMapper>();
                cfg.AddProfile<ApiMapper>();
            }, new LoggerFactory());
        config.AssertConfigurationIsValid();
    }
}
