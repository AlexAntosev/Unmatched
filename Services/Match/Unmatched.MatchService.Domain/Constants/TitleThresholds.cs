namespace Unmatched.MatchService.Domain.Constants;

/// <summary>Qualification thresholds for the per-match (Shared) title rules - kept separate from
/// <see cref="RatingConstants"/> since these tune title eligibility, not the rating ladder itself.</summary>
public static class TitleThresholds
{
    /// <summary>Rusher: win with at least this fraction of the deck still unplayed.</summary>
    public const double RusherMinCardRatio = 0.66;

    /// <summary>Punisher: a single ranked match must swing the winner's rating by at least this many
    /// points. Retuned from the old "MatchPoints >= 1000", which is unreachable under Elo's K=32 scale.</summary>
    public const int PunisherMinMatchPoints = 45;

    /// <summary>Last Breath: win with at most this much HP left.</summary>
    public const int LastBreathMaxHp = 2;

    /// <summary>Giant Slayer: beat an opponent whose pre-match rating was at least this many points
    /// higher.</summary>
    public const int GiantSlayerRatingGap = 300;
}
