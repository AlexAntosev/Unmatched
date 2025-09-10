namespace Unmatched.PlayerService.Tests
{
    using AutoMapper;

    using Microsoft.Extensions.Logging;

    using Unmatched.PlayerService.Api.Mapping;

    public class MapperTests
    {
        [Fact]
        public void AssertMappings()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ApiMapper>(), new LoggerFactory());
            config.AssertConfigurationIsValid();
        }
    }
}
