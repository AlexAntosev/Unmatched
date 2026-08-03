namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Tournaments;

public class BountyChampionshipTests
{
    [Fact]
    public void Compute_NoMatchesPlayed_ChampionIsTheStartingChampion()
    {
        var startingChampion = Guid.NewGuid();

        var state = BountyChampionship.Compute(startingChampion, []);

        Assert.Equal(startingChampion, state.ChampionHeroId);
        Assert.Equal(0, state.DefenseCount);
    }

    [Fact]
    public void Compute_NoStartingChampionAndNoMatches_ChampionIsNull()
    {
        var state = BountyChampionship.Compute(null, []);

        Assert.Null(state.ChampionHeroId);
        Assert.Equal(0, state.DefenseCount);
    }

    [Fact]
    public void Compute_ChampionDefendsSuccessfully_DefenseCountIncrements()
    {
        var champion = Guid.NewGuid();
        var challenger = Guid.NewGuid();
        var matches = new List<MatchEntity>
        {
            CreateMatch(new DateTime(2026, 1, 1), winnerId: champion, loserId: challenger),
            CreateMatch(new DateTime(2026, 1, 8), winnerId: champion, loserId: Guid.NewGuid()),
        };

        var state = BountyChampionship.Compute(champion, matches);

        Assert.Equal(champion, state.ChampionHeroId);
        Assert.Equal(2, state.DefenseCount);
    }

    [Fact]
    public void Compute_ChallengerWins_ChampionshipTransfersAndDefenseCountResets()
    {
        var startingChampion = Guid.NewGuid();
        var newChampion = Guid.NewGuid();
        var matches = new List<MatchEntity>
        {
            CreateMatch(new DateTime(2026, 1, 1), winnerId: startingChampion, loserId: Guid.NewGuid()),
            CreateMatch(new DateTime(2026, 1, 8), winnerId: newChampion, loserId: startingChampion),
        };

        var state = BountyChampionship.Compute(startingChampion, matches);

        Assert.Equal(newChampion, state.ChampionHeroId);
        Assert.Equal(0, state.DefenseCount);
    }

    [Fact]
    public void Compute_MatchesOutOfInsertionOrder_ReplaysByDateNotByListOrder()
    {
        var startingChampion = Guid.NewGuid();
        var newChampion = Guid.NewGuid();

        // inserted Feb-then-Jan, but the championship must transfer in Jan and be re-defended in Feb.
        var matches = new List<MatchEntity>
        {
            CreateMatch(new DateTime(2026, 2, 1), winnerId: newChampion, loserId: Guid.NewGuid()),
            CreateMatch(new DateTime(2026, 1, 1), winnerId: newChampion, loserId: startingChampion),
        };

        var state = BountyChampionship.Compute(startingChampion, matches);

        Assert.Equal(newChampion, state.ChampionHeroId);
        Assert.Equal(1, state.DefenseCount);
    }

    [Fact]
    public void Compute_IgnoresPlannedMatches()
    {
        var champion = Guid.NewGuid();
        var challenger = Guid.NewGuid();
        var plannedUpset = CreateMatch(new DateTime(2026, 1, 1), winnerId: challenger, loserId: champion);
        plannedUpset.IsPlanned = true;

        var state = BountyChampionship.Compute(champion, [plannedUpset]);

        Assert.Equal(champion, state.ChampionHeroId);
        Assert.Equal(0, state.DefenseCount);
    }

    private static MatchEntity CreateMatch(DateTime date, Guid winnerId, Guid loserId)
        => new()
        {
            Date = date,
            IsPlanned = false,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerId, IsWinner = true },
                new() { HeroId = loserId, IsWinner = false },
            },
        };
}
