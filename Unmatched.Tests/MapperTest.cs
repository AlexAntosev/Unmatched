namespace Unmatched.Tests;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Unmatched.Mapping;

public class MapperTest
{
    [Fact]
    public void CheckMappings()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UnmatchedMapper>(), new LoggerFactory());
        config.AssertConfigurationIsValid();
    }
}
