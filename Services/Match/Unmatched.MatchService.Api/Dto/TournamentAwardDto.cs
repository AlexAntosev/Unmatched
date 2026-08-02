namespace Unmatched.MatchService.Api.Dto;

using Unmatched.MatchService.Domain.Enums;

public class TournamentAwardDto
{
    public Guid HeroId { get; set; }

    public TournamentAwardKind AwardKind { get; set; }

    public int Points { get; set; }

    public DateTime AwardedAt { get; set; }
}
