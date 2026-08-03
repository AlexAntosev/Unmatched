namespace Unmatched.MatchService.Domain.Models;

/// <summary>The live state of a Bounty tournament, derived from its challenge history - see
/// <see cref="Tournaments.BountyChampionship"/>. Null <see cref="ChampionHeroId"/> means the pool has
/// no starting champion assigned yet.</summary>
public class BountyState
{
    public Guid? ChampionHeroId { get; set; }

    public int DefenseCount { get; set; }
}
