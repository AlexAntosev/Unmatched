namespace Unmatched.MatchService.Api.Dto;

using Unmatched.MatchService.Domain.Enums;

public class RatingChangeDto
{
    public DateTime Date { get; set; }

    public int RatingDelta { get; set; }

    public int PointsChange { get; set; }

    public bool IsAward { get; set; }

    public bool? IsWin { get; set; }

    public Guid? OpponentHeroId { get; set; }

    public Guid? TournamentId { get; set; }

    public TournamentAwardKind? AwardKind { get; set; }
}
