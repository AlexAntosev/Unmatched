namespace Unmatched.CatalogService.Tests
{
    using AutoMapper;

    using Microsoft.Extensions.Logging;

    using Unmatched.CatalogService.Api.Dto;
    using Unmatched.CatalogService.Api.Mapping;
    using Unmatched.CatalogService.Domain.Entities;

    public class MapperTests
    {
        private static readonly MapperConfiguration Config = new(cfg => cfg.AddProfile<ApiMapper>(), new LoggerFactory());

        [Fact]
        public void AssertMappings()
        {
            Config.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_HeroWithExpansion_ProjectsExpansionName()
        {
            var hero = new Hero
            {
                Name = "Achilles",
                Color = "#B56647",
                Sidekicks = [],
                Expansion = new Expansion { Name = "Battle of Legends: Volume One" }
            };

            var dto = Config.CreateMapper().Map<HeroDto>(hero);

            Assert.Equal("Battle of Legends: Volume One", dto.ExpansionName);
        }

        [Fact]
        public void Map_HeroWithoutExpansion_ExpansionNameIsNull()
        {
            var hero = new Hero
            {
                Name = "Achilles",
                Color = "#B56647",
                Sidekicks = [],
                Expansion = null
            };

            var dto = Config.CreateMapper().Map<HeroDto>(hero);

            Assert.Null(dto.ExpansionName);
        }

        [Fact]
        public void Map_MapWithoutExpansion_ExpansionNameIsNull()
        {
            var map = new Map { Name = "Troy", Expansion = null };

            var dto = Config.CreateMapper().Map<MapDto>(map);

            Assert.Null(dto.ExpansionName);
        }
    }
}
