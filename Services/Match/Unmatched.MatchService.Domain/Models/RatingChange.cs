namespace Unmatched.MatchService.Domain.Models;

using Unmatched.MatchService.Domain.Enums;

public class RatingChange
{
    public DateTime Date { get; set; }

    public int RatingDelta { get; set; }

    /// <summary>The points this single event moved the rating by, as opposed to
    /// <see cref="RatingDelta"/> which is the running total after it.</summary>
    public int PointsChange { get; set; }

    public bool IsAward { get; set; }

    /// <summary>Set only for match-derived events (<see cref="IsAward"/> is false).</summary>
    public bool? IsWin { get; set; }

    /// <summary>Best-effort opponent for match-derived events - the first fighter on the match whose
    /// hero differs from the one this history belongs to.</summary>
    public Guid? OpponentHeroId { get; set; }

    public Guid? TournamentId { get; set; }

    /// <summary>Set only for award-derived events (<see cref="IsAward"/> is true).</summary>
    public TournamentAwardKind? AwardKind { get; set; }
}
