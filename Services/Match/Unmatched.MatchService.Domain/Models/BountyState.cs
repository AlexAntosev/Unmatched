namespace Unmatched.MatchService.Domain.Models;

/// <summary>The live state of a Bounty tournament. <see cref="ChampionHeroId"/>/<see cref="BankPoints"/>
/// read straight off the tournament's single <see cref="Entities.TournamentAwardEntity"/> pool row (see
/// <see cref="RatingCalculators.BountyRatingCalculator"/>); <see cref="DefenseCount"/> is the one part
/// still derived by replaying match history (see <see cref="Tournaments.BountyChampionship"/>), since
/// there is no persisted counter for it. Null <see cref="ChampionHeroId"/> means the pool has no starting
/// champion assigned yet.</summary>
public class BountyState
{
    public Guid? ChampionHeroId { get; set; }

    public int DefenseCount { get; set; }

    public int BankPoints { get; set; }
}
