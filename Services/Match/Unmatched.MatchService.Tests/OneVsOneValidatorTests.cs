namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Validation;

public class OneVsOneValidatorTests
{
    private readonly OneVsOneValidator _validator = new();

    [Fact]
    public void Validate_ExactlyTwoFightersOneWinner_DoesNotThrow()
    {
        var match = new MatchEntity
            {
                Fighters = new List<FighterEntity>
                    {
                        new() { IsWinner = true },
                        new() { IsWinner = false }
                    }
            };

        var exception = Record.Exception(() => _validator.Validate(match));

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_ThreeFighters_ThrowsException()
    {
        var match = new MatchEntity
            {
                Fighters = new List<FighterEntity>
                    {
                        new() { IsWinner = true },
                        new() { IsWinner = false },
                        new() { IsWinner = false }
                    }
            };

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_NoWinner_ThrowsException()
    {
        var match = new MatchEntity
            {
                Fighters = new List<FighterEntity>
                    {
                        new() { IsWinner = false },
                        new() { IsWinner = false }
                    }
            };

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_BothWinners_ThrowsException()
    {
        var match = new MatchEntity
            {
                Fighters = new List<FighterEntity>
                    {
                        new() { IsWinner = true },
                        new() { IsWinner = true }
                    }
            };

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }
}
