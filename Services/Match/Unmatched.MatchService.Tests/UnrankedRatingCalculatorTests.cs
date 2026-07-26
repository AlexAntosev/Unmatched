namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;

public class UnrankedRatingCalculatorTests
{
    private readonly UnrankedRatingCalculator _calculator = new();

    [Fact]
    public async Task CalculateAsync_AwardsFlatPointsRegardlessOfMatchStats()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        var winner = new FighterEntity { HeroId = winnerHeroId, IsWinner = true, HpLeft = 0, CardsLeft = 0, SidekickHpLeft = 0 };
        var looser = new FighterEntity { HeroId = looserHeroId, IsWinner = false, HpLeft = 99, CardsLeft = 99, SidekickHpLeft = 99 };

        var result = await _calculator.CalculateAsync(winner, looser);

        Assert.Equal(250, result[winnerHeroId]);
        Assert.Equal(-150, result[looserHeroId]);
    }

    [Fact]
    public async Task CalculateAsync_IdentifiesWinnerByIsWinnerFlagRegardlessOfArgumentOrder()
    {
        var winnerHeroId = Guid.NewGuid();
        var looserHeroId = Guid.NewGuid();

        var winner = new FighterEntity { HeroId = winnerHeroId, IsWinner = true };
        var looser = new FighterEntity { HeroId = looserHeroId, IsWinner = false };

        // pass the looser as the first ("fighter") argument
        var result = await _calculator.CalculateAsync(looser, winner);

        Assert.Equal(250, result[winnerHeroId]);
        Assert.Equal(-150, result[looserHeroId]);
    }
}
