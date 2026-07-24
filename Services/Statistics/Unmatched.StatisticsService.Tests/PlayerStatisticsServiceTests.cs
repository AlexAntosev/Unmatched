namespace Unmatched.StatisticsService.Tests;

using Moq;

using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Communication.Player.Http;
using Unmatched.StatisticsService.Domain.Communication.Player.Http.Dto;
using Unmatched.StatisticsService.Domain.Services;

public class PlayerStatisticsServiceTests
{
    private readonly Mock<IMatchClient> _matchClient = new();
    private readonly Mock<IPlayerCache> _playerCache = new();

    private readonly PlayerStatisticsService _service;

    public PlayerStatisticsServiceTests()
    {
        _service = new PlayerStatisticsService(_matchClient.Object, _playerCache.Object);
    }

    [Fact]
    public async Task GetPlayerStatisticsAsync_ComputesWinsLoosesAndLastMatchPoints()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var opponentId = Guid.NewGuid();

        var matches = new List<MatchLogDto>
            {
                CreateMatch(new DateTime(2024, 1, 1), Fighter(playerId, isWinner: true, matchPoints: 100), Fighter(opponentId, isWinner: false, matchPoints: 50)),
                CreateMatch(new DateTime(2024, 2, 1), Fighter(playerId, isWinner: false, matchPoints: -20), Fighter(opponentId, isWinner: true, matchPoints: 20)),
                CreateMatch(new DateTime(2024, 3, 1), Fighter(playerId, isWinner: true, matchPoints: 40), Fighter(opponentId, isWinner: false, matchPoints: -10))
            };

        _playerCache.Setup(c => c.GetAsync(playerId)).ReturnsAsync(new PlayerDto { Id = playerId, Name = "Andrii" });
        _matchClient.Setup(c => c.GetFinishedByPlayerAsync(playerId)).ReturnsAsync(matches);

        // Act
        var stats = await _service.GetPlayerStatisticsAsync(playerId);

        // Assert
        Assert.Equal(playerId, stats.PlayerId);
        Assert.Equal("Andrii", stats.Name);
        Assert.Equal(3, stats.TotalMatches);
        Assert.Equal(2, stats.TotalWins);
        Assert.Equal(1, stats.TotalLooses);
        Assert.Equal(40, stats.LastMatchPoints); // most recent match by date, not by list order
    }

    [Fact]
    public async Task GetPlayerStatisticsAsync_NoMatches_ReturnsZeroedStats()
    {
        // Arrange
        var playerId = Guid.NewGuid();

        _playerCache.Setup(c => c.GetAsync(playerId)).ReturnsAsync(new PlayerDto { Id = playerId, Name = "Andrii" });
        _matchClient.Setup(c => c.GetFinishedByPlayerAsync(playerId)).ReturnsAsync(new List<MatchLogDto>());

        // Act
        var stats = await _service.GetPlayerStatisticsAsync(playerId);

        // Assert
        Assert.Equal(0, stats.TotalMatches);
        Assert.Equal(0, stats.TotalWins);
        Assert.Equal(0, stats.TotalLooses);
        Assert.Equal(0, stats.LastMatchPoints);
        Assert.Equal(0, stats.Kd);
    }

    [Fact]
    public async Task GetPlayerStatisticsAsync_NoLosses_DoesNotDivideByZero()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var opponentId = Guid.NewGuid();

        var matches = new List<MatchLogDto>
            {
                CreateMatch(new DateTime(2024, 1, 1), Fighter(playerId, isWinner: true, matchPoints: 100), Fighter(opponentId, isWinner: false, matchPoints: 50))
            };

        _playerCache.Setup(c => c.GetAsync(playerId)).ReturnsAsync(new PlayerDto { Id = playerId, Name = "Andrii" });
        _matchClient.Setup(c => c.GetFinishedByPlayerAsync(playerId)).ReturnsAsync(matches);

        // Act
        var stats = await _service.GetPlayerStatisticsAsync(playerId);

        // Assert
        Assert.Equal(0, stats.TotalLooses);
        Assert.Equal(0, stats.Kd);
    }

    [Fact]
    public async Task GetPlayersStatisticsAsync_SplitsSharedMatchLogByPlayer()
    {
        // Arrange
        var playerOneId = Guid.NewGuid();
        var playerTwoId = Guid.NewGuid();

        var matches = new List<MatchLogDto>
            {
                CreateMatch(new DateTime(2024, 1, 1), Fighter(playerOneId, isWinner: true, matchPoints: 100), Fighter(playerTwoId, isWinner: false, matchPoints: -100))
            };

        _playerCache.Setup(c => c.GetAsync()).ReturnsAsync(
            new List<PlayerDto>
                {
                    new() { Id = playerOneId, Name = "Andrii" },
                    new() { Id = playerTwoId, Name = "Olex" }
                });
        _matchClient.Setup(c => c.GetMatchLogAsync()).ReturnsAsync(matches);

        // Act
        var stats = (await _service.GetPlayersStatisticsAsync()).ToList();

        // Assert
        var playerOneStats = stats.Single(x => x.PlayerId == playerOneId);
        var playerTwoStats = stats.Single(x => x.PlayerId == playerTwoId);

        Assert.Equal(1, playerOneStats.TotalWins);
        Assert.Equal(0, playerOneStats.TotalLooses);
        Assert.Equal(0, playerTwoStats.TotalWins);
        Assert.Equal(1, playerTwoStats.TotalLooses);
    }

    private static MatchLogDto CreateMatch(DateTime date, params FighterDto[] fighters)
    {
        return new MatchLogDto
            {
                Date = date,
                Fighters = fighters
            };
    }

    private static FighterDto Fighter(Guid playerId, bool isWinner, int matchPoints)
    {
        return new FighterDto
            {
                IsWinner = isWinner,
                MatchPoints = matchPoints,
                Player = new FighterPlayerDto { Id = playerId, Name = "Player" }
            };
    }
}
