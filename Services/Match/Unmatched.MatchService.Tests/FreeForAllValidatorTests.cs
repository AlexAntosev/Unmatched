namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Validation;

public class FreeForAllValidatorTests
{
    private readonly FreeForAllValidator _validator = new();

    private static MatchEntity ValidThreePlayerMatch() => new()
        {
            Fighters = new List<FighterEntity>
                {
                    new() { Placement = 1, IsWinner = true },
                    new() { Placement = 2, IsWinner = false },
                    new() { Placement = 3, IsWinner = false }
                }
        };

    [Fact]
    public void Validate_ValidThreePlayerMatch_DoesNotThrow()
    {
        var match = ValidThreePlayerMatch();

        var exception = Record.Exception(() => _validator.Validate(match));

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_OnlyTwoFighters_ThrowsException()
    {
        var match = new MatchEntity
            {
                Fighters = new List<FighterEntity>
                    {
                        new() { Placement = 1, IsWinner = true },
                        new() { Placement = 2, IsWinner = false }
                    }
            };

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_DuplicatePlacements_ThrowsException()
    {
        var match = ValidThreePlayerMatch();
        match.Fighters.Last().Placement = 2;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_WinnerFlagNotMatchingFirstPlace_ThrowsException()
    {
        var match = ValidThreePlayerMatch();
        match.Fighters.First().IsWinner = false;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_MissingPlacement_ThrowsException()
    {
        var match = ValidThreePlayerMatch();
        match.Fighters.First().Placement = null;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }
}
