namespace Unmatched.StatisticsService.Tests;

using Moq;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;
using Unmatched.StatisticsService.Domain.Communication.Match.Kafka;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class MatchCreatedMinionHandlerTests
{
    private readonly Mock<ICatalogMinionCache> _catalogMinionCache = new();
    private readonly Mock<IMinionStatsRepository> _minionStatsRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly MatchCreatedMinionHandler _handler;

    public MatchCreatedMinionHandlerTests()
    {
        _unitOfWork.Setup(u => u.MinionStats).Returns(_minionStatsRepository.Object);
        _handler = new MatchCreatedMinionHandler(_catalogMinionCache.Object);
    }

    [Fact]
    public async Task HandleAsync_NoVillainOnEvent_DoesNothing()
    {
        var matchCreated = new MatchCreated { Fighters = new List<MatchCreated.Fighter>(), Villain = null };

        await _handler.HandleAsync(_unitOfWork.Object, matchCreated);

        _minionStatsRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<MinionStats>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_MultipleMinions_UpdatesEachIndependently()
    {
        var minionAId = Guid.NewGuid();
        var minionBId = Guid.NewGuid();
        var matchCreated = new MatchCreated
            {
                Date = new DateTime(2026, 1, 1),
                Fighters = new List<MatchCreated.Fighter>(),
                Villain = new MatchCreated.MatchVillain
                    {
                        VillainId = Guid.NewGuid(),
                        IsWinner = false,
                        Minions = new List<MatchCreated.MatchMinion>
                            {
                                new() { MinionId = minionAId, Name = "Bandit", IsWinner = false },
                                new() { MinionId = minionBId, Name = "Cultist", IsWinner = false }
                            }
                    }
            };

        _minionStatsRepository.Setup(r => r.GetByMinionAsync(minionAId)).ReturnsAsync((MinionStats?)null);
        _minionStatsRepository.Setup(r => r.GetByMinionAsync(minionBId)).ReturnsAsync((MinionStats?)null);
        _catalogMinionCache.Setup(c => c.GetAsync(minionAId)).ReturnsAsync(new CatalogMinionDto { Id = minionAId, Name = "Bandit" });
        _catalogMinionCache.Setup(c => c.GetAsync(minionBId)).ReturnsAsync(new CatalogMinionDto { Id = minionBId, Name = "Cultist" });

        var savedStats = new List<MinionStats>();
        _minionStatsRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<MinionStats>()))
            .Callback<MinionStats>(s => savedStats.Add(s))
            .Returns(Task.CompletedTask);

        await _handler.HandleAsync(_unitOfWork.Object, matchCreated);

        Assert.Equal(2, savedStats.Count);
        Assert.All(savedStats, s => Assert.Equal(1, s.TotalMatches));
        Assert.All(savedStats, s => Assert.Equal(1, s.TotalLooses));
        Assert.Contains(savedStats, s => s.Name == "Bandit");
        Assert.Contains(savedStats, s => s.Name == "Cultist");
    }
}
