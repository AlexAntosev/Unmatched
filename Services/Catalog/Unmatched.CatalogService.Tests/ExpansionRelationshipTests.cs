namespace Unmatched.CatalogService.Tests
{
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using Unmatched.CatalogService.Domain.Entities;
    using Unmatched.CatalogService.EntityFramework.Context;

    public class ExpansionRelationshipTests
    {
        private static UnmatchedDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new UnmatchedDbContext(options);
        }

        // The InMemory provider ignores schema-level constraints (unique indexes, FKs) entirely, so
        // uniqueness must be verified against a provider that actually enforces relational constraints.
        private static (UnmatchedDbContext Context, SqliteConnection Connection) CreateSqliteContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
                .UseSqlite(connection)
                .Options;
            var context = new UnmatchedDbContext(options);
            context.Database.EnsureCreated();
            return (context, connection);
        }

        [Fact]
        public async Task DeletingExpansion_SetsHeroExpansionIdToNull()
        {
            await using var context = CreateContext();
            var expansion = new Expansion { Name = "The Witcher: Deadly Alliances" };
            var hero = new Hero { Name = "Geralt of Rivia", Color = "#DBDEE6", Sidekicks = [], Expansion = expansion };
            context.Expansions.Add(expansion);
            context.Heroes.Add(hero);
            await context.SaveChangesAsync();

            context.Expansions.Remove(expansion);
            await context.SaveChangesAsync();

            var reloaded = await context.Heroes.AsNoTracking().SingleAsync(h => h.Id == hero.Id);
            Assert.Null(reloaded.ExpansionId);
        }

        [Fact]
        public async Task DeletingExpansion_SetsMapExpansionIdToNull()
        {
            await using var context = CreateContext();
            var expansion = new Expansion { Name = "The Witcher: Deadly Alliances" };
            var map = new Map { Name = "Kaer Morhen", Expansion = expansion };
            context.Expansions.Add(expansion);
            context.Maps.Add(map);
            await context.SaveChangesAsync();

            context.Expansions.Remove(expansion);
            await context.SaveChangesAsync();

            var reloaded = await context.Maps.AsNoTracking().SingleAsync(m => m.Id == map.Id);
            Assert.Null(reloaded.ExpansionId);
        }

        [Fact]
        public async Task Hero_WithNullExpansion_SavesAndLoadsSuccessfully()
        {
            await using var context = CreateContext();
            var hero = new Hero { Name = "Achilles", Color = "#B56647", Sidekicks = [] };
            context.Heroes.Add(hero);
            await context.SaveChangesAsync();

            var reloaded = await context.Heroes.AsNoTracking().SingleAsync(h => h.Id == hero.Id);
            Assert.Null(reloaded.ExpansionId);
        }

        [Fact]
        public async Task DuplicateExpansionName_ThrowsOnSave()
        {
            var (context, connection) = CreateSqliteContext();
            await using var _ = context;
            await using var __ = connection;

            context.Expansions.Add(new Expansion { Name = "Cobble & Fog" });
            await context.SaveChangesAsync();

            context.Expansions.Add(new Expansion { Name = "Cobble & Fog" });

            await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }
    }
}
