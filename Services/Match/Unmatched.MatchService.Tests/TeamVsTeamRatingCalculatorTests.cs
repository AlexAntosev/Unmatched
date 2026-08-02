namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

public class TeamVsTeamRatingCalculatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    private readonly TeamVsTeamRatingCalculator _calculator;

    public TeamVsTeamRatingCalculatorTests()
    {
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _calculator = new TeamVsTeamRatingCalculator(_unitOfWork.Object, _catalogHeroCache.Object);
    }

    [Fact]
    public async Task CalculateAsync_EveryMemberOfAWinningTeamGetsTheSameDelta()
    {
        var (winnerAId, winnerBId, loserAId, loserBId) = SetUpEqualTeams();

        var result = await _calculator.CalculateAsync(CreateMatch(winnerAId, winnerBId, loserAId, loserBId));

        Assert.Equal(result[winnerAId], result[winnerBId]);
        Assert.Equal(result[loserAId], result[loserBId]);
    }

    [Fact]
    public async Task CalculateAsync_IsZeroSum()
    {
        var (winnerAId, winnerBId, loserAId, loserBId) = SetUpEqualTeams();

        var result = await _calculator.CalculateAsync(CreateMatch(winnerAId, winnerBId, loserAId, loserBId));

        Assert.Equal(0, result[winnerAId] + result[winnerBId] + result[loserAId] + result[loserBId]);
    }

    [Fact]
    public async Task CalculateAsync_UnderdogTeamWins_GetsMoreThanTheSameWinWouldPayAFavouriteTeam()
    {
        var winnerAId = Guid.NewGuid();
        var winnerBId = Guid.NewGuid();
        var loserAId = Guid.NewGuid();
        var loserBId = Guid.NewGuid();
        foreach (var heroId in new[] { winnerAId, winnerBId, loserAId, loserBId })
        {
            SetUpHero(heroId);
        }

        SetUpRating(winnerAId, 800);
        SetUpRating(winnerBId, 800);
        SetUpRating(loserAId, 1200);
        SetUpRating(loserBId, 1200);
        var underdogResult = await _calculator.CalculateAsync(CreateMatch(winnerAId, winnerBId, loserAId, loserBId));

        SetUpRating(winnerAId, 1200);
        SetUpRating(winnerBId, 1200);
        SetUpRating(loserAId, 800);
        SetUpRating(loserBId, 800);
        var favouriteResult = await _calculator.CalculateAsync(CreateMatch(winnerAId, winnerBId, loserAId, loserBId));

        Assert.True(underdogResult[winnerAId] > favouriteResult[winnerAId]);
    }

    private (Guid WinnerAId, Guid WinnerBId, Guid LoserAId, Guid LoserBId) SetUpEqualTeams()
    {
        var winnerAId = Guid.NewGuid();
        var winnerBId = Guid.NewGuid();
        var loserAId = Guid.NewGuid();
        var loserBId = Guid.NewGuid();
        foreach (var heroId in new[] { winnerAId, winnerBId, loserAId, loserBId })
        {
            SetUpHero(heroId);
            SetUpRating(heroId, 1000);
        }

        return (winnerAId, winnerBId, loserAId, loserBId);
    }

    private static MatchEntity CreateMatch(Guid winnerAId, Guid winnerBId, Guid loserAId, Guid loserBId)
        => new()
        {
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerAId, IsWinner = true, Team = 1, HpLeft = 16, SidekickHpLeft = 0, CardsLeft = 10 },
                new() { HeroId = winnerBId, IsWinner = true, Team = 1, HpLeft = 16, SidekickHpLeft = 0, CardsLeft = 10 },
                new() { HeroId = loserAId, IsWinner = false, Team = 2, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 },
                new() { HeroId = loserBId, IsWinner = false, Team = 2, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 },
            },
        };

    private void SetUpHero(Guid heroId)
        => _catalogHeroCache
            .Setup(c => c.GetAsync(heroId))
            .ReturnsAsync(new CatalogHeroDto { Id = heroId, Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() });

    private void SetUpRating(Guid heroId, int points)
        => _ratingRepository.Setup(r => r.GetByHeroIdAsync(heroId)).ReturnsAsync(new RatingEntity { HeroId = heroId, Points = points });
}
