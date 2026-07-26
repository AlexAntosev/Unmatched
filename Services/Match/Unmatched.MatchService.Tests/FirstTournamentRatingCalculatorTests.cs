namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.RatingCalculators;

public class FirstTournamentRatingCalculatorTests
{
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    private readonly FirstTournamentRatingCalculator _calculator;

    private static readonly Guid WinnerHeroId = Guid.NewGuid();
    private static readonly Guid LooserHeroId = Guid.NewGuid();

    public FirstTournamentRatingCalculatorTests()
    {
        _catalogHeroCache
            .Setup(c => c.GetAsync())
            .ReturnsAsync(new[]
            {
                new CatalogHeroDto { Id = WinnerHeroId, Hp = 100 },
                new CatalogHeroDto { Id = LooserHeroId, Hp = 100 },
            });

        _calculator = new FirstTournamentRatingCalculator(_catalogHeroCache.Object);
    }

    [Theory]
    [InlineData(Stage.Group, 160)]
    [InlineData(Stage.SixteenthFinals, 80)]
    [InlineData(Stage.EighthFinals, 80)]
    [InlineData(Stage.QuarterFinals, 640)]
    [InlineData(Stage.SemiFinals, 640)]
    [InlineData(Stage.ThirdPlaceDecider, 640)]
    [InlineData(Stage.GrandFinals, 320)]
    public async Task CalculateAsync_WinnerPointsAreScaledByStageCoefficient(Stage stage, int expectedWinnerPoints)
    {
        var winner = new FighterEntity { HeroId = WinnerHeroId, IsWinner = true, HpLeft = 0 };
        var looser = new FighterEntity { HeroId = LooserHeroId, IsWinner = false, HpLeft = 0 };

        var result = await _calculator.CalculateAsync(winner, looser, stage);

        Assert.Equal(expectedWinnerPoints, result[WinnerHeroId]);
    }

    [Theory]
    [InlineData(0, 80)]
    [InlineData(50, 100)]
    [InlineData(100, 120)]
    public async Task CalculateAsync_WinnerPointsScaleWithHpLeftRatio(int hpLeft, int expectedWinnerPoints)
    {
        var winner = new FighterEntity { HeroId = WinnerHeroId, IsWinner = true, HpLeft = hpLeft };
        var looser = new FighterEntity { HeroId = LooserHeroId, IsWinner = false, HpLeft = 0 };

        var result = await _calculator.CalculateAsync(winner, looser, Stage.EighthFinals);

        Assert.Equal(expectedWinnerPoints, result[WinnerHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_NullHpLeftIsTreatedAsZero()
    {
        var winner = new FighterEntity { HeroId = WinnerHeroId, IsWinner = true, HpLeft = null };
        var looser = new FighterEntity { HeroId = LooserHeroId, IsWinner = false, HpLeft = 0 };

        var result = await _calculator.CalculateAsync(winner, looser, Stage.EighthFinals);

        Assert.Equal(80, result[WinnerHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_LooserAlwaysGetsZeroPoints()
    {
        var winner = new FighterEntity { HeroId = WinnerHeroId, IsWinner = true, HpLeft = 100 };
        var looser = new FighterEntity { HeroId = LooserHeroId, IsWinner = false, HpLeft = 0 };

        var result = await _calculator.CalculateAsync(winner, looser, Stage.GrandFinals);

        Assert.Equal(0, result[LooserHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_IdentifiesWinnerByIsWinnerFlagRegardlessOfArgumentOrder()
    {
        var winner = new FighterEntity { HeroId = WinnerHeroId, IsWinner = true, HpLeft = 0 };
        var looser = new FighterEntity { HeroId = LooserHeroId, IsWinner = false, HpLeft = 0 };

        // pass the looser as the first ("fighter") argument
        var result = await _calculator.CalculateAsync(looser, winner, Stage.EighthFinals);

        Assert.Equal(80, result[WinnerHeroId]);
        Assert.Equal(0, result[LooserHeroId]);
    }
}
