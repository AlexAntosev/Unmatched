namespace Unmatched.CatalogService.Tests
{
    using AutoMapper;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Unmatched.CatalogService.Api.Dto;
    using Unmatched.CatalogService.Api.Mapping;
    using Unmatched.CatalogService.Domain.Entities;
    using Unmatched.CatalogService.Domain.Services;
    using Unmatched.CatalogService.EntityFramework.Context;
    using Unmatched.CatalogService.EntityFramework.Repositories;

    /// <summary>
    /// The collection screen needs everything that ships in a box, so the expansion endpoint has to
    /// return heroes, maps, villains and minions - not just the first two.
    /// </summary>
    public class ExpansionContentTests
    {
        private static readonly MapperConfiguration Config = new(cfg => cfg.AddProfile<ApiMapper>(), new LoggerFactory());

        private static UnmatchedDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new UnmatchedDbContext(options);
        }

        private static async Task<UnmatchedDbContext> SeededContextAsync()
        {
            var context = CreateContext();
            var expansion = new Expansion { Name = "Jurassic Park: InGen vs Raptors", ReleaseYear = 2021 };
            context.Expansions.Add(expansion);
            context.Heroes.Add(new Hero { Name = "Dr. Sattler", Color = "#4B7F52", Sidekicks = [], Expansion = expansion });
            context.Heroes.Add(new Hero { Name = "Raptors", Color = "#7F4B4B", Sidekicks = [], Expansion = expansion });
            context.Maps.Add(new Map { Name = "Raptor Paddock", Expansion = expansion });
            context.Villains.Add(new Villain { Name = "InGen", Color = "#333333", Hp = 20, DeckSize = 30, Expansion = expansion });
            context.Minions.Add(new Minion { Name = "Raptor", Color = "#555555", Hp = 4, DeckSize = 0, Expansion = expansion });
            context.Minions.Add(new Minion { Name = "Guard", Color = "#666666", Hp = 3, DeckSize = 0, Expansion = expansion });
            await context.SaveChangesAsync();
            return context;
        }

        [Fact]
        public async Task GetAsync_IncludesVillainsAndMinions()
        {
            await using var context = await SeededContextAsync();
            var repository = new ExpansionRepository(context);

            var expansion = Assert.Single(await repository.GetAsync());

            Assert.Equal(2, expansion.Heroes.Count);
            Assert.Single(expansion.Maps);
            Assert.Single(expansion.Villains);
            Assert.Equal(2, expansion.Minions.Count);
        }

        [Fact]
        public async Task Map_Expansion_ProjectsEveryContentCollection()
        {
            await using var context = await SeededContextAsync();
            var repository = new ExpansionRepository(context);
            var expansion = (await repository.GetAsync()).Single();

            var dto = Config.CreateMapper().Map<ExpansionDto>(expansion);

            Assert.Equal(["Dr. Sattler", "Raptors"], dto.Heroes.Select(h => h.Name).Order());
            Assert.Equal(["Raptor Paddock"], dto.Maps.Select(m => m.Name));
            Assert.Equal(["InGen"], dto.Villains.Select(v => v.Name));
            Assert.Equal(["Guard", "Raptor"], dto.Minions.Select(m => m.Name).Order());
        }

        [Fact]
        public async Task Map_Villain_CarriesHpAndDeckSize()
        {
            await using var context = await SeededContextAsync();
            var repository = new ExpansionRepository(context);
            var expansion = (await repository.GetAsync()).Single();

            var villain = Config.CreateMapper().Map<ExpansionDto>(expansion).Villains.Single();

            Assert.Equal(20, villain.Hp);
            Assert.Equal(30, villain.DeckSize);
        }

        [Fact]
        public async Task UpdateImageAsync_StoresCover()
        {
            await using var context = await SeededContextAsync();
            var unitOfWork = new UnitOfWork(context);
            var service = new ExpansionService(unitOfWork);
            var id = (await unitOfWork.Expansions.GetAsync()).Single().Id;

            var updated = await service.UpdateImageAsync(id, "jurassic-park.png");

            Assert.NotNull(updated);
            Assert.Equal("jurassic-park.png", updated!.ImageFileName);
        }

        [Fact]
        public async Task UpdateImageAsync_UnknownExpansion_ReturnsNull()
        {
            await using var context = await SeededContextAsync();
            var service = new ExpansionService(new UnitOfWork(context));

            Assert.Null(await service.UpdateImageAsync(Guid.NewGuid(), "whatever.png"));
        }
    }
}
