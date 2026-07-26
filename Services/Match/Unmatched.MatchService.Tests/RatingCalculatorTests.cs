namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

public class RatingCalculatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    private readonly RatingCalculator _calculator;

    public RatingCalculatorTests()
    {
        _unitOfWork.Setup(uow => uow.Ratings).Returns(_ratingRepository.Object);
        _calculator = new RatingCalculator(_unitOfWork.Object, _catalogHeroCache.Object);
    }

    [Fact]
    public async Task CalculateAsync_FullHealthNoSidekicksNoHandicap_AwardsMaximumBaseBonuses()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        SetupHero(winnerHeroId, hp: 16, deckSize: 10);
        SetupHero(looserHeroId, hp: 16, deckSize: 10);
        SetupRatings(winnerHeroId, 0, looserHeroId, 0);

        var winner = new FighterEntity
        {
            HeroId = winnerHeroId,
            IsWinner = true,
            HpLeft = 16,
            SidekickHpLeft = 0,
            CardsLeft = 8,
        };
        var looser = new FighterEntity
        {
            HeroId = looserHeroId,
            IsWinner = false,
            HpLeft = 0,
            SidekickHpLeft = 0,
        };

        var result = await _calculator.CalculateAsync(winner, looser);

        Assert.Equal(400, result[winnerHeroId]);
        Assert.Equal(-300, result[looserHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_LooserHadHigherRating_HandicapScalesProportionallyToPointGap()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        SetupHero(winnerHeroId, hp: 16, deckSize: 10);
        SetupHero(looserHeroId, hp: 16, deckSize: 10, sidekickHp: 10, sidekickCount: 1);
        SetupRatings(winnerHeroId, 1000, looserHeroId, 1250);

        var winner = new FighterEntity
        {
            HeroId = winnerHeroId,
            IsWinner = true,
            HpLeft = 0,
            SidekickHpLeft = 0,
            CardsLeft = 0,
        };
        var looser = new FighterEntity
        {
            HeroId = looserHeroId,
            IsWinner = false,
            HpLeft = 0,
            SidekickHpLeft = 10,
        };

        var result = await _calculator.CalculateAsync(winner, looser);

        // base 200 + 0 (cards) + 0 (hp left) + 0 (looser sidekick fully alive) + handicap for a 250-point gap.
        // Before the fix this used integer division (250 / 500 == 0), so a sub-500 gap was worth nothing at all.
        Assert.Equal(250, result[winnerHeroId]);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(250, 50)]
    [InlineData(450, 90)]
    [InlineData(499, 100)]
    [InlineData(500, 100)]
    [InlineData(1000, 200)]
    public async Task CalculateAsync_HandicapIsContinuousAcrossPointGap_NoStepAt500Multiples(int pointGap, int expectedHandicap)
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        SetupHero(winnerHeroId, hp: 16, deckSize: 10);
        SetupHero(looserHeroId, hp: 16, deckSize: 10, sidekickHp: 10, sidekickCount: 1);
        SetupRatings(winnerHeroId, 1000, looserHeroId, 1000 + pointGap);

        var winner = new FighterEntity
        {
            HeroId = winnerHeroId,
            IsWinner = true,
            HpLeft = 0,
            SidekickHpLeft = 0,
            CardsLeft = 0,
        };
        var looser = new FighterEntity
        {
            HeroId = looserHeroId,
            IsWinner = false,
            HpLeft = 0,
            SidekickHpLeft = 10,
        };

        var result = await _calculator.CalculateAsync(winner, looser);

        Assert.Equal(200 + expectedHandicap, result[winnerHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_WinnerHadHigherRating_NoHandicapAwarded()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        SetupHero(winnerHeroId, hp: 16, deckSize: 10);
        SetupHero(looserHeroId, hp: 16, deckSize: 10, sidekickHp: 10, sidekickCount: 1);
        SetupRatings(winnerHeroId, 1500, looserHeroId, 1000);

        var winner = new FighterEntity
        {
            HeroId = winnerHeroId,
            IsWinner = true,
            HpLeft = 0,
            SidekickHpLeft = 0,
            CardsLeft = 0,
        };
        var looser = new FighterEntity
        {
            HeroId = looserHeroId,
            IsWinner = false,
            HpLeft = 0,
            SidekickHpLeft = 10,
        };

        var result = await _calculator.CalculateAsync(winner, looser);

        Assert.Equal(200, result[winnerHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_PartialSidekickDamage_ScalesBonusAndPenaltyBySurvivingHp()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        SetupHero(winnerHeroId, hp: 20, deckSize: 10);
        SetupHero(looserHeroId, hp: 16, deckSize: 10, sidekickHp: 10, sidekickCount: 2);
        SetupRatings(winnerHeroId, 0, looserHeroId, 0);

        var winner = new FighterEntity
        {
            HeroId = winnerHeroId,
            IsWinner = true,
            HpLeft = 20,
            SidekickHpLeft = 0,
            CardsLeft = 0,
        };
        var looser = new FighterEntity
        {
            HeroId = looserHeroId,
            IsWinner = false,
            HpLeft = 0,
            SidekickHpLeft = 8, // 8 / 20 max sidekick HP = 0.4 surviving
        };

        var result = await _calculator.CalculateAsync(winner, looser);

        // winner: 200 base + 24 (40 * (1 - 0.4) sidekick bonus) + 0 cards + 0 handicap + 80 (full winner HP left)
        Assert.Equal(304, result[winnerHeroId]);
        // looser: -(100 base + 30 (50 * (1 - 0.4) sidekick penalty) + 150 (winner at full HP))
        Assert.Equal(-280, result[looserHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_NoLooserSidekicks_UsesFlatFullBonusInsteadOfDividingByZero()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        SetupHero(winnerHeroId, hp: 16, deckSize: 10);
        SetupHero(looserHeroId, hp: 16, deckSize: 10); // no sidekicks at all
        SetupRatings(winnerHeroId, 0, looserHeroId, 0);

        var winner = new FighterEntity
        {
            HeroId = winnerHeroId,
            IsWinner = true,
            HpLeft = 0,
            SidekickHpLeft = 0,
            CardsLeft = 0,
        };
        var looser = new FighterEntity
        {
            HeroId = looserHeroId,
            IsWinner = false,
            HpLeft = 0,
            SidekickHpLeft = 0,
        };

        var result = await _calculator.CalculateAsync(winner, looser);

        Assert.Equal(240, result[winnerHeroId]); // 200 + flat 40 sidekick bonus
        Assert.Equal(-150, result[looserHeroId]); // -(100 + flat 50 sidekick penalty)
    }

    [Fact]
    public async Task CalculateAsync_IdentifiesWinnerByIsWinnerFlagRegardlessOfArgumentOrder()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        SetupHero(winnerHeroId, hp: 16, deckSize: 10);
        SetupHero(looserHeroId, hp: 16, deckSize: 10);
        SetupRatings(winnerHeroId, 0, looserHeroId, 0);

        var winner = new FighterEntity
        {
            HeroId = winnerHeroId,
            IsWinner = true,
            HpLeft = 0,
            SidekickHpLeft = 0,
            CardsLeft = 0,
        };
        var looser = new FighterEntity
        {
            HeroId = looserHeroId,
            IsWinner = false,
            HpLeft = 0,
            SidekickHpLeft = 0,
        };

        // pass the looser as the first ("fighter") argument
        var result = await _calculator.CalculateAsync(looser, winner);

        Assert.True(result[winnerHeroId] > 0);
        Assert.True(result[looserHeroId] < 0);
    }

    private void SetupHero(Guid heroId, int hp, int deckSize, int sidekickHp = 0, int sidekickCount = 0)
    {
        var sidekicks = sidekickCount > 0
            ? new[] { new CatalogSidekickDto { Hp = sidekickHp, Count = sidekickCount } }
            : Array.Empty<CatalogSidekickDto>();

        _catalogHeroCache
            .Setup(c => c.GetAsync(heroId))
            .ReturnsAsync(new CatalogHeroDto { Id = heroId, Hp = hp, DeckSize = deckSize, Sidekicks = sidekicks });
    }

    private void SetupRatings(Guid winnerHeroId, int winnerPoints, Guid looserHeroId, int looserPoints)
    {
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(winnerHeroId)).ReturnsAsync(new RatingEntity { HeroId = winnerHeroId, Points = winnerPoints });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(looserHeroId)).ReturnsAsync(new RatingEntity { HeroId = looserHeroId, Points = looserPoints });
    }
}
