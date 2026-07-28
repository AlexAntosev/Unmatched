namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Mapping;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

using Title = Unmatched.MatchService.Domain.Models.Title;

/// <summary>
/// These exercise TitleService against a real EF Core context (in-memory provider) rather than mocks,
/// because the bug this used to have - HeroTitles never actually persisting on Assign/Unassign/Merge -
/// only shows up with real EF change tracking. A repository fetched with AsNoTracking() lets you mutate
/// its in-memory HeroTitles collection all day without SaveChangesAsync ever noticing.
/// </summary>
public class TitleServiceTests : IDisposable
{
    private readonly UnmatchedDbContext _dbContext;
    private readonly TitleService _titleService;
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    public TitleServiceTests()
    {
        var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new UnmatchedDbContext(options);

        var unitOfWork = new TestUnitOfWork(_dbContext);
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DomainMapper>(), new LoggerFactory()).CreateMapper();

        _titleService = new TitleService(unitOfWork, mapper, _catalogHeroCache.Object);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task MergeAsync_AddsNewHeroAssignment_AndPersistsIt()
    {
        var titleId = await SeedTitleAsync("Streak");
        var heroId = Guid.NewGuid();

        await _titleService.MergeAsync(titleId, new[] { heroId });

        var persisted = await RequeryHeroIdsAsync(titleId);
        Assert.Equal(new[] { heroId }, persisted);
    }

    [Fact]
    public async Task MergeAsync_RemovesHeroNoLongerInList_AndPersistsRemoval()
    {
        var titleId = await SeedTitleAsync("Streak");
        var heroId = Guid.NewGuid();
        await _titleService.MergeAsync(titleId, new[] { heroId });

        await _titleService.MergeAsync(titleId, Array.Empty<Guid>());

        var persisted = await RequeryHeroIdsAsync(titleId);
        Assert.Empty(persisted);
    }

    [Fact]
    public async Task MergeAsync_ReplacesAssignmentsWithNewSet()
    {
        var titleId = await SeedTitleAsync("Streak");
        var keptHeroId = Guid.NewGuid();
        var removedHeroId = Guid.NewGuid();
        var addedHeroId = Guid.NewGuid();
        await _titleService.MergeAsync(titleId, new[] { keptHeroId, removedHeroId });

        await _titleService.MergeAsync(titleId, new[] { keptHeroId, addedHeroId });

        var persisted = await RequeryHeroIdsAsync(titleId);
        Assert.Equal(new[] { keptHeroId, addedHeroId }.OrderBy(x => x), persisted.OrderBy(x => x));
    }

    [Fact]
    public async Task AssignAsync_AddsHeroToTitle_AndPersistsIt()
    {
        var titleId = await SeedTitleAsync("Streak");
        var heroId = Guid.NewGuid();
        _catalogHeroCache.Setup(c => c.GetAsync(heroId)).ReturnsAsync(new CatalogHeroDto { Id = heroId });

        await _titleService.AssignAsync(titleId, heroId);

        var persisted = await RequeryHeroIdsAsync(titleId);
        Assert.Equal(new[] { heroId }, persisted);
    }

    [Fact]
    public async Task UnassignAsync_RemovesHeroFromTitle_AndPersistsRemoval()
    {
        var titleId = await SeedTitleAsync("Streak");
        var heroId = Guid.NewGuid();
        _catalogHeroCache.Setup(c => c.GetAsync(heroId)).ReturnsAsync(new CatalogHeroDto { Id = heroId });
        await _titleService.AssignAsync(titleId, heroId);

        await _titleService.UnassignAsync(titleId, heroId);

        var persisted = await RequeryHeroIdsAsync(titleId);
        Assert.Empty(persisted);
    }

    [Fact]
    public async Task GetHeroesForTitleAssign_MarksAssignedHeroesAsIsAssignedTrue()
    {
        var titleId = await SeedTitleAsync("Streak");
        var assignedHeroId = Guid.NewGuid();
        var unassignedHeroId = Guid.NewGuid();
        await _titleService.MergeAsync(titleId, new[] { assignedHeroId });

        _catalogHeroCache.Setup(c => c.GetAsync()).ReturnsAsync(new[]
        {
            new CatalogHeroDto { Id = assignedHeroId, Name = "Assigned Hero" },
            new CatalogHeroDto { Id = unassignedHeroId, Name = "Unassigned Hero" },
        });

        var result = (await _titleService.GetHeroesForTitleAssign(titleId)).ToList();

        Assert.True(result.Single(h => h.Id == assignedHeroId).IsAssigned);
        Assert.False(result.Single(h => h.Id == unassignedHeroId).IsAssigned);
    }

    [Fact]
    public async Task GetByHeroAsync_ReturnsEveryTitleTheHeroHolds()
    {
        var heroId = Guid.NewGuid();
        var streakId = await SeedTitleAsync("Streak");
        var punisherId = await SeedTitleAsync("Punisher");
        await SeedTitleAsync("Rusher");
        await _titleService.MergeAsync(streakId, new[] { heroId });
        await _titleService.MergeAsync(punisherId, new[] { heroId });

        var titles = (await _titleService.GetByHeroAsync(heroId)).Select(t => t.Name).Order().ToList();

        Assert.Equal(new[] { "Punisher", "Streak" }, titles);
    }

    [Fact]
    public async Task GetByHeroAsync_HeroWithoutTitles_ReturnsNothing()
    {
        var titleId = await SeedTitleAsync("Streak");
        await _titleService.MergeAsync(titleId, new[] { Guid.NewGuid() });

        Assert.Empty(await _titleService.GetByHeroAsync(Guid.NewGuid()));
    }

    private async Task<Guid> SeedTitleAsync(string name)
    {
        var entity = new TitleEntity { Id = Guid.NewGuid(), Name = name, Comment = string.Empty, HeroTitles = new List<HeroTitleEntity>() };
        _dbContext.Titles.Add(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
        return entity.Id;
    }

    private async Task<List<Guid>> RequeryHeroIdsAsync(Guid titleId)
    {
        return await _dbContext.HeroTitles.Where(ht => ht.TitlesId == titleId).Select(ht => ht.HeroesId).ToListAsync();
    }

    private class TestUnitOfWork(UnmatchedDbContext dbContext) : IUnitOfWork
    {
        public IFighterRepository Fighters => throw new NotImplementedException();

        public IMatchRepository Matches => throw new NotImplementedException();

        public IRatingRepository Ratings => throw new NotImplementedException();

        public IRatingRecalculationStateRepository RatingRecalculationState => throw new NotImplementedException();

        public ITournamentRepository Tournaments => throw new NotImplementedException();

        public ITitleRepository Titles { get; } = new TitleRepository(dbContext);

        public IHeroTitleRepository HeroTitles => throw new NotImplementedException();

        public Task SaveChangesAsync() => dbContext.SaveChangesAsync();

        public void Dispose()
        {
        }
    }
}
