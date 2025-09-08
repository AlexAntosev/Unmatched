namespace Unmatched.CatalogService.Tests
{
    using AutoMapper;

    using Microsoft.Extensions.Logging;

    using Unmatched.CatalogService.Api.Mapping;

    public class MapperTests
    {
        [Fact]
        public void AssertMappings()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CatalogMapper>(), new LoggerFactory());
            config.AssertConfigurationIsValid();
        }
    }
}
