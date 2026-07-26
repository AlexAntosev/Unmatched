namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Validation;

public class TeamVsTeamValidatorTests
{
    private readonly TeamVsTeamValidator _validator = new();

    private static MatchEntity ValidTwoVsTwoMatch() => new()
        {
            Fighters = new List<FighterEntity>
                {
                    new() { Team = 1, IsWinner = true },
                    new() { Team = 1, IsWinner = true },
                    new() { Team = 2, IsWinner = false },
                    new() { Team = 2, IsWinner = false }
                }
        };

    [Fact]
    public void Validate_ValidTwoVsTwo_DoesNotThrow()
    {
        var match = ValidTwoVsTwoMatch();

        var exception = Record.Exception(() => _validator.Validate(match));

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_MissingTeamAssignment_ThrowsException()
    {
        var match = ValidTwoVsTwoMatch();
        match.Fighters.First().Team = null;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_UnevenTeamSizes_ThrowsException()
    {
        var match = new MatchEntity
            {
                Fighters = new List<FighterEntity>
                    {
                        new() { Team = 1, IsWinner = true },
                        new() { Team = 2, IsWinner = false },
                        new() { Team = 2, IsWinner = false }
                    }
            };

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_InconsistentResultWithinTeam_ThrowsException()
    {
        var match = ValidTwoVsTwoMatch();
        match.Fighters.First().IsWinner = false;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_BothTeamsWin_ThrowsException()
    {
        var match = ValidTwoVsTwoMatch();
        foreach (var fighter in match.Fighters)
        {
            fighter.IsWinner = true;
        }

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }
}
