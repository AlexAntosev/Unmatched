namespace Unmatched.Dtos.Match;

using Unmatched.Enums;

public class TournamentAwardDto
{
    public Guid HeroId { get; set; }

    public TournamentAwardKind AwardKind { get; set; }

    public int Points { get; set; }

    public DateTime AwardedAt { get; set; }
}
