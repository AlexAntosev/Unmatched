namespace Unmatched.StatisticsService.Tests;

using Moq;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;
using Unmatched.StatisticsService.Domain.Communication.Match.Kafka;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class MatchCreatedVillainHandlerTests
{
    private readonly Mock<ICatalogVillainCache> _catalogVillainCache = new();
    private readonly Mock<IVillainStatsRepository> _villainStatsRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly MatchCreatedVillainHandler _handler;

    public MatchCreatedVillainHandlerTests()
    {
        _unitOfWork.Setup(u => u.VillainStats).Returns(_villainStatsRepository.Object);
        _handler = new MatchCreatedVillainHandler(_catalogVillainCache.Object);
    }

    [Fact]
    public async Task HandleAsync_NoVillainOnEvent_DoesNothing()
    {
        var matchCreated = new MatchCreated { Fighters = new List<MatchCreated.Fighter>(), Villain = null };

        await _handler.HandleAsync(_unitOfWork.Object, matchCreated);

        _villainStatsRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<VillainStats>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_NewVillain_CreatesEntryFromCatalogAndCountsFirstMatch()
    {
        var villainId = Guid.NewGuid();
        var matchCreated = new MatchCreated
            {
                Date = new DateTime(2026, 1, 1),
                Fighters = new List<MatchCreated.Fighter>(),
                Villain = new MatchCreated.MatchVillain { VillainId = villainId, Name = "Matango", IsWinner = false }
            };

        _villainStatsRepository.Setup(r => r.GetByVillainAsync(villainId)).ReturnsAsync((VillainStats?)null);
        _catalogVillainCache.Setup(c => c.GetAsync(villainId)).ReturnsAsync(new CatalogVillainDto { Id = villainId, Name = "Matango", Hp = 20, DeckSize = 15, Color = "green" });

        VillainStats? savedStats = null;
        _villainStatsRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<VillainStats>()))
            .Callback<VillainStats>(s => savedStats = s)
            .Returns(Task.CompletedTask);

        await _handler.HandleAsync(_unitOfWork.Object, matchCreated);

        Assert.NotNull(savedStats);
        Assert.Equal("Matango", savedStats!.Name);
        Assert.Equal(20, savedStats.Hp);
        Assert.Equal(1, savedStats.TotalMatches);
        Assert.Equal(0, savedStats.TotalWins);
        Assert.Equal(1, savedStats.TotalLooses);
    }

    [Fact]
    public async Task HandleAsync_ExistingVillainWins_IncrementsWinsNotLooses()
    {
        var villainId = Guid.NewGuid();
        var matchCreated = new MatchCreated
            {
                Date = new DateTime(2026, 1, 1),
                Fighters = new List<MatchCreated.Fighter>(),
                Villain = new MatchCreated.MatchVillain { VillainId = villainId, Name = "Matango", IsWinner = true }
            };

        var existingStats = new VillainStats { VillainId = villainId, Name = "Matango", TotalMatches = 2, TotalWins = 1, TotalLooses = 1 };
        _villainStatsRepository.Setup(r => r.GetByVillainAsync(villainId)).ReturnsAsync(existingStats);

        await _handler.HandleAsync(_unitOfWork.Object, matchCreated);

        Assert.Equal(3, existingStats.TotalMatches);
        Assert.Equal(2, existingStats.TotalWins);
        Assert.Equal(1, existingStats.TotalLooses);
    }
}
