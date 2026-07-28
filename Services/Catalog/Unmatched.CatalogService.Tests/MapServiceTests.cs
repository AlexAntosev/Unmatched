namespace Unmatched.CatalogService.Tests
{
    using Microsoft.EntityFrameworkCore;

    using Unmatched.CatalogService.Domain.Entities;
    using Unmatched.CatalogService.Domain.Services;
    using Unmatched.CatalogService.EntityFramework.Context;
    using Unmatched.CatalogService.EntityFramework.Repositories;

    public class MapServiceTests
    {
        private static async Task<UnmatchedDbContext> SeededContextAsync()
        {
            var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new UnmatchedDbContext(options);

            context.Maps.Add(new Map { Name = "Raptor Paddock" });
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task UpdateImageAsync_StoresImage()
        {
            await using var context = await SeededContextAsync();
            var unitOfWork = new UnitOfWork(context);
            var service = new MapService(unitOfWork);
            var id = (await unitOfWork.Maps.GetAsync()).Single().Id;

            var updated = await service.UpdateImageAsync(id, "raptor-paddock.png");

            Assert.NotNull(updated);
            Assert.Equal("raptor-paddock.png", updated!.ImageFileName);
        }

        [Fact]
        public async Task UpdateImageAsync_UnknownMap_ReturnsNull()
        {
            await using var context = await SeededContextAsync();
            var service = new MapService(new UnitOfWork(context));

            Assert.Null(await service.UpdateImageAsync(Guid.NewGuid(), "whatever.png"));
        }
    }
}
