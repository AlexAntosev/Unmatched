namespace Unmatched.MatchService.Domain.Enums;

/// <summary>
/// Which tier of the completion-bonus schedule a <see cref="Entities.TournamentAwardEntity"/> row
/// represents. Named after the bracket round a participant was eliminated at, since that is what the
/// payout actually keys off (see <see cref="Tournaments.TournamentAwardScheduler"/>) - Swiss and League
/// reuse the same tiers for their own placement-based schedules rather than introduce parallel names for
/// an identical concept ("came in 2nd" pays the same as "lost the grand final").
/// </summary>
public enum TournamentAwardKind
{
    Winner,
    Finalist,
    Semifinalist,
    Quarterfinalist,
    EighthFinalist,
    SixteenthFinalist,

    /// <summary>GroupStage only - a flat bonus for topping a group, awarded in addition to whatever
    /// bracket tier the group's advancement earns.</summary>
    GroupWinner,

    /// <summary>League's own three-tier scale (see <see cref="Constants.RatingConstants.LeagueFirstAwardMultiplier"/>).</summary>
    SeasonFirst,
    SeasonSecond,
    SeasonThird,

    /// <summary>No tier bonus earned - the entry fee still applies, so this row's points are negative.</summary>
    Eliminated,

    /// <summary>Bounty only - exactly one row per Bounty tournament, upserted (never appended) by
    /// <see cref="RatingCalculators.BountyRatingCalculator"/> as challenges resolve. HeroId is the current
    /// title holder, Points is the bank accumulated during this holder's reign. Unlike every other kind
    /// here, this row's Points is a running balance, not a rating delta - it is deliberately excluded from
    /// <see cref="Services.RatingTimeline.BuildAsync"/> so it never gets summed and applied as one.
    /// Appended last so its int value never collides with an existing persisted row.</summary>
    BountyPool
}
