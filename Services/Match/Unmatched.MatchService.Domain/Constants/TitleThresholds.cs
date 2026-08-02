namespace Unmatched.MatchService.Domain.Constants;

/// <summary>Qualification thresholds for the per-match (Shared) title rules - kept separate from
/// <see cref="RatingConstants"/> since these tune title eligibility, not the rating ladder itself.</summary>
public static class TitleThresholds
{
    /// <summary>Last Breath: win with at most this much HP left.</summary>
    public const int LastBreathMaxHp = 2;

    /// <summary>Giant Slayer: beat an opponent whose pre-match rating was at least this many points
    /// higher.</summary>
    public const int GiantSlayerRatingGap = 300;
}
