namespace Unmatched.StatisticsService.Tests;

using Moq;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.Domain.Services;

public class ExpansionStatisticsServiceTests
{
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();
    private readonly Mock<IHeroStatsRepository> _heroStatsRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly ExpansionStatisticsService _service;

    public ExpansionStatisticsServiceTests()
    {
        _unitOfWork.Setup(u => u.HeroStats).Returns(_heroStatsRepository.Object);
        _service = new ExpansionStatisticsService(_catalogHeroCache.Object, _unitOfWork.Object);
    }

    private void Setup(IEnumerable<CatalogHeroDto> heroes, IEnumerable<HeroStats> stats)
    {
        _catalogHeroCache.Setup(c => c.GetAsync()).ReturnsAsync(heroes);
        _heroStatsRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(stats.ToList());
    }

    private static CatalogHeroDto Hero(Guid id, string name, Guid? expansionId)
        => new() { Id = id, Name = name, Color = "#fff", Sidekicks = [], ExpansionId = expansionId };

    private static HeroStats Stats(Guid id, string name, int matches, int wins, int losses, int points)
        => new()
            {
                HeroId = id,
                Name = name,
                Color = "#fff",
                Points = points,
                TotalMatches = matches,
                TotalWins = wins,
                TotalLooses = losses
            };

    [Fact]
    public async Task GetExpansionsStatisticsAsync_SumsHeroStatsPerExpansion()
    {
        var expansionId = Guid.NewGuid();
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        Setup(
            [Hero(first, "Sherlock Holmes", expansionId), Hero(second, "Dracula", expansionId)],
            [Stats(first, "Sherlock Holmes", 10, 7, 3, 40), Stats(second, "Dracula", 6, 2, 4, 10)]);

        var stats = Assert.Single(await _service.GetExpansionsStatisticsAsync());

        Assert.Equal(expansionId, stats.ExpansionId);
        Assert.Equal(16, stats.TotalMatches);
        Assert.Equal(9, stats.TotalWins);
        Assert.Equal(7, stats.TotalLooses);
    }

    [Fact]
    public async Task GetExpansionsStatisticsAsync_BestHeroIsTheOneWithMostPoints()
    {
        var expansionId = Guid.NewGuid();
        var sherlock = Guid.NewGuid();
        var dracula = Guid.NewGuid();
        Setup(
            [Hero(sherlock, "Sherlock Holmes", expansionId), Hero(dracula, "Dracula", expansionId)],
            [Stats(sherlock, "Sherlock Holmes", 10, 7, 3, 40), Stats(dracula, "Dracula", 6, 2, 4, 10)]);

        var stats = Assert.Single(await _service.GetExpansionsStatisticsAsync());

        Assert.Equal(sherlock, stats.BestHeroId);
        Assert.Equal("Sherlock Holmes", stats.BestHeroName);
    }

    [Fact]
    public async Task GetExpansionsStatisticsAsync_RoundsWinRateToWholePercent()
    {
        var expansionId = Guid.NewGuid();
        var heroId = Guid.NewGuid();
        Setup([Hero(heroId, "Alice", expansionId)], [Stats(heroId, "Alice", 3, 2, 1, 5)]);

        var stats = Assert.Single(await _service.GetExpansionsStatisticsAsync());

        Assert.Equal(67, stats.WinRate);
    }

    [Fact]
    public async Task GetExpansionsStatisticsAsync_IgnoresHeroesWithoutExpansion()
    {
        var heroId = Guid.NewGuid();
        Setup([Hero(heroId, "Homebrew", expansionId: null)], [Stats(heroId, "Homebrew", 4, 2, 2, 5)]);

        Assert.Empty(await _service.GetExpansionsStatisticsAsync());
    }

    [Fact]
    public async Task GetExpansionsStatisticsAsync_ExpansionWithNoPlayedHeroes_ReportsZeroesAndNoBestHero()
    {
        var expansionId = Guid.NewGuid();
        // The hero exists in the catalog but has never been played, so it has no stats row.
        Setup([Hero(Guid.NewGuid(), "Unplayed", expansionId)], []);

        var stats = Assert.Single(await _service.GetExpansionsStatisticsAsync());

        Assert.Equal(0, stats.TotalMatches);
        Assert.Equal(0, stats.WinRate);
        Assert.Null(stats.BestHeroId);
        Assert.Null(stats.BestHeroName);
    }

    [Fact]
    public async Task GetExpansionsStatisticsAsync_KeepsExpansionsApart()
    {
        var cobbleAndFog = Guid.NewGuid();
        var battleOfLegends = Guid.NewGuid();
        var sherlock = Guid.NewGuid();
        var achilles = Guid.NewGuid();
        Setup(
            [Hero(sherlock, "Sherlock Holmes", cobbleAndFog), Hero(achilles, "Achilles", battleOfLegends)],
            [Stats(sherlock, "Sherlock Holmes", 10, 7, 3, 40), Stats(achilles, "Achilles", 4, 1, 3, 2)]);

        var stats = (await _service.GetExpansionsStatisticsAsync()).ToDictionary(s => s.ExpansionId);

        Assert.Equal(10, stats[cobbleAndFog].TotalMatches);
        Assert.Equal(4, stats[battleOfLegends].TotalMatches);
    }
}
