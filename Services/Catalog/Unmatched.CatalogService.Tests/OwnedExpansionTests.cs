namespace Unmatched.CatalogService.Tests
{
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using Unmatched.CatalogService.Domain.Entities;
    using Unmatched.CatalogService.Domain.Services;
    using Unmatched.CatalogService.EntityFramework.Context;
    using Unmatched.CatalogService.EntityFramework.Repositories;

    public class OwnedExpansionTests
    {
        private static UnmatchedDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new UnmatchedDbContext(options);
        }

        // The InMemory provider ignores schema-level constraints (unique indexes, FKs) entirely, so
        // constraint enforcement must be verified against a provider that actually enforces them.
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
        public async Task DuplicateExpansionId_ThrowsOnSave()
        {
            var (context, connection) = CreateSqliteContext();
            await using var _ = context;
            await using var __ = connection;

            var expansion = new Expansion { Name = "Cobble & Fog" };
            context.Expansions.Add(expansion);
            await context.SaveChangesAsync();

            context.OwnedExpansions.Add(new OwnedExpansion { ExpansionId = expansion.Id });
            await context.SaveChangesAsync();

            context.OwnedExpansions.Add(new OwnedExpansion { ExpansionId = expansion.Id });

            await Assert.ThrowsAnyAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task DeletingExpansion_DeletesOwnedExpansionRow()
        {
            var (context, connection) = CreateSqliteContext();
            await using var _ = context;
            await using var __ = connection;

            var expansion = new Expansion { Name = "Cobble & Fog" };
            context.Expansions.Add(expansion);
            context.OwnedExpansions.Add(new OwnedExpansion { ExpansionId = expansion.Id });
            await context.SaveChangesAsync();

            context.Expansions.Remove(expansion);
            await context.SaveChangesAsync();

            var remaining = await context.OwnedExpansions.AsNoTracking().ToListAsync();
            Assert.Empty(remaining);
        }

        [Fact]
        public async Task ReplaceAsync_ReplacesPreviousSelectionEntirely()
        {
            await using var context = CreateContext();
            var expansionA = new Expansion { Name = "Cobble & Fog" };
            var expansionB = new Expansion { Name = "Battle of Legends, Volume One" };
            context.Expansions.AddRange(expansionA, expansionB);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var service = new OwnedExpansionService(unitOfWork);

            await service.ReplaceAsync([expansionA.Id, expansionB.Id]);
            await service.ReplaceAsync([expansionB.Id]);

            var owned = await service.GetOwnedExpansionIdsAsync();
            Assert.Equal([expansionB.Id], owned);
        }

        [Fact]
        public async Task GetOwnedExpansionIdsAsync_WithNoSelection_ReturnsEmpty()
        {
            await using var context = CreateContext();
            var unitOfWork = new UnitOfWork(context);
            var service = new OwnedExpansionService(unitOfWork);

            var owned = await service.GetOwnedExpansionIdsAsync();

            Assert.Empty(owned);
        }
    }
}
