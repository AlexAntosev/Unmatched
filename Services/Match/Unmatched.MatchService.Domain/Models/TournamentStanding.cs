namespace Unmatched.MatchService.Domain.Models;

/// <summary>One participant's record within a tournament - works the same way for League, Swiss and
/// the group phase of GroupStage. The UI resolves hero name/art by joining <see cref="HeroId"/>
/// against the hero catalog it already has loaded.</summary>
public class TournamentStanding
{
    public Guid HeroId { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int? FinalPlacement { get; set; }
}
