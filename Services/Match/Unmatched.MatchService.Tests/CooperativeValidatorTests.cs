namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Validation;

public class CooperativeValidatorTests
{
    private readonly CooperativeValidator _validator = new();

    private static MatchEntity ValidPlayersWinMatch() => new()
        {
            Fighters = new List<FighterEntity>
                {
                    new() { IsWinner = true },
                    new() { IsWinner = true }
                },
            Villain = new MatchVillainEntity
                {
                    IsWinner = false,
                    Minions = new List<MatchMinionEntity>
                        {
                            new() { IsWinner = false }
                        }
                }
        };

    [Fact]
    public void Validate_PlayersWinUnanimouslyAgainstVillain_DoesNotThrow()
    {
        var match = ValidPlayersWinMatch();

        var exception = Record.Exception(() => _validator.Validate(match));

        Assert.Null(exception);
    }

    [Fact]
    public void Validate_NoVillain_ThrowsException()
    {
        var match = ValidPlayersWinMatch();
        match.Villain = null;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_FightersDisagreeOnResult_ThrowsException()
    {
        var match = ValidPlayersWinMatch();
        match.Fighters.First().IsWinner = false;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_VillainResultMatchesPlayers_ThrowsException()
    {
        var match = ValidPlayersWinMatch();
        match.Villain!.IsWinner = true; // both players and Villain "won" - contradiction

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }

    [Fact]
    public void Validate_MinionResultDoesNotMatchVillain_ThrowsException()
    {
        var match = ValidPlayersWinMatch();
        match.Villain!.Minions.First().IsWinner = true;

        Assert.Throws<ArgumentException>(() => _validator.Validate(match));
    }
}
