namespace Unmatched.MatchService.Domain.Models;

using Unmatched.MatchService.Domain.Enums;

public class TournamentAward
{
    public Guid HeroId { get; set; }

    public TournamentAwardKind AwardKind { get; set; }

    public int Points { get; set; }

    public DateTime AwardedAt { get; set; }
}
