namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

public class OneVsOneRatingCalculatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    private readonly OneVsOneRatingCalculator _calculator;

    public OneVsOneRatingCalculatorTests()
    {
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _calculator = new OneVsOneRatingCalculator(_unitOfWork.Object, _catalogHeroCache.Object);
    }

    [Fact]
    public async Task CalculateAsync_IsZeroSum()
    {
        var (winnerId, looserId) = SetUpEqualMatch();

        var result = await _calculator.CalculateAsync(CreateMatch(winnerId, looserId));

        Assert.Equal(-result[winnerId], result[looserId]);
    }

    [Fact]
    public async Task CalculateAsync_NoRatingRowsYet_TreatsBothHeroesAsStartingAtInitialRating()
    {
        var (winnerId, looserId) = SetUpEqualMatch();
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(It.IsAny<Guid>())).ReturnsAsync((RatingEntity?)null);

        var result = await _calculator.CalculateAsync(CreateMatch(winnerId, looserId));

        Assert.Equal(RatingConstants.KFactor / 2, result[winnerId]);
    }

    [Fact]
    public async Task CalculateAsync_UnderdogWins_GetsMoreThanTheSameWinWouldPayAFavourite()
    {
        var winnerId = Guid.NewGuid();
        var looserId = Guid.NewGuid();
        SetUpHero(winnerId, hp: 16, deckSize: 10);
        SetUpHero(looserId, hp: 16, deckSize: 10);

        SetUpRating(winnerId, 800);
        SetUpRating(looserId, 1200);
        var underdogWinResult = await _calculator.CalculateAsync(CreateMatch(winnerId, looserId));

        SetUpRating(winnerId, 1200);
        SetUpRating(looserId, 800);
        var favouriteWinResult = await _calculator.CalculateAsync(CreateMatch(winnerId, looserId));

        Assert.True(underdogWinResult[winnerId] > favouriteWinResult[winnerId]);
    }

    [Fact]
    public async Task CalculateAsync_IdentifiesWinnerByIsWinnerFlag_RegardlessOfFighterOrder()
    {
        var winnerId = Guid.NewGuid();
        var looserId = Guid.NewGuid();
        SetUpHero(winnerId, hp: 16, deckSize: 10);
        SetUpHero(looserId, hp: 16, deckSize: 10);
        SetUpRating(winnerId, 1000);
        SetUpRating(looserId, 1000);

        var match = new MatchEntity
        {
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = looserId, IsWinner = false, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 },
                new() { HeroId = winnerId, IsWinner = true, HpLeft = 16, SidekickHpLeft = 0, CardsLeft = 10 },
            },
        };

        var result = await _calculator.CalculateAsync(match);

        Assert.True(result[winnerId] > 0);
        Assert.True(result[looserId] < 0);
    }

    private (Guid WinnerId, Guid LooserId) SetUpEqualMatch()
    {
        var winnerId = Guid.NewGuid();
        var looserId = Guid.NewGuid();
        SetUpHero(winnerId, hp: 16, deckSize: 10);
        SetUpHero(looserId, hp: 16, deckSize: 10);
        SetUpRating(winnerId, 1000);
        SetUpRating(looserId, 1000);
        return (winnerId, looserId);
    }

    /// <summary>Half HP/cards for the winner and half cards for the loser (both sidekick-less heroes)
    /// lands the performance modifier at exactly 1.0 - a "typical" win, deliberately not a crush, so
    /// tests can assert a clean K/2-based delta instead of a formula-dependent one.</summary>
    private static MatchEntity CreateMatch(Guid winnerId, Guid looserId)
        => new()
        {
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerId, IsWinner = true, HpLeft = 8, SidekickHpLeft = 0, CardsLeft = 5 },
                new() { HeroId = looserId, IsWinner = false, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 5 },
            },
        };

    private void SetUpHero(Guid heroId, int hp, int deckSize)
        => _catalogHeroCache
            .Setup(c => c.GetAsync(heroId))
            .ReturnsAsync(new CatalogHeroDto { Id = heroId, Hp = hp, DeckSize = deckSize, Sidekicks = Array.Empty<CatalogSidekickDto>() });

    private void SetUpRating(Guid heroId, int points)
        => _ratingRepository.Setup(r => r.GetByHeroIdAsync(heroId)).ReturnsAsync(new RatingEntity { HeroId = heroId, Points = points });
}
