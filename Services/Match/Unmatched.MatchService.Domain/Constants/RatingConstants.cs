namespace Unmatched.MatchService.Domain.Constants;

/// <summary>
/// Every tunable number behind the Elo-based rating system lives here, so retuning the ladder
/// (K-factor, how much margin-of-victory can swing a result, how a tournament payout scales) never
/// requires touching the calculators themselves.
/// </summary>
public static class RatingConstants
{
    /// <summary>Starting rating for a hero (or, once Phase 4 uses it, a tournament participant) that
    /// has no rating row yet.</summary>
    public const int InitialRating = 1000;

    /// <summary>Maximum points a single ranked match can move a rating by. Uniform for every hero in
    /// every match - there is no provisional/placement phase, so the ladder stays strictly zero-sum
    /// without needing to average a per-match K.</summary>
    public const int KFactor = 32;

    /// <summary>Standard Elo denominator: a 400-point rating gap corresponds to a 10:1 expected-score
    /// ratio.</summary>
    public const int EloScale = 400;

    /// <summary>The performance modifier (see <see cref="RatingCalculators.PerformanceModifier"/>)
    /// ranges over [PerformanceModifierBase, PerformanceModifierBase + PerformanceModifierRange].</summary>
    public const double PerformanceModifierBase = 0.75;

    public const double PerformanceModifierRange = 0.5;

    /// <summary>Weights for the winning side's own remaining resources. Must sum to 1.</summary>
    public const double OwnHpWeight = 0.40;

    public const double OwnSidekickWeight = 0.25;

    public const double OwnCardWeight = 0.35;

    /// <summary>Weights for the losing side's remaining resources. Its own HP is excluded - it is
    /// always 0, since reducing it to 0 is how the match was won - so only sidekicks and cards
    /// carry information. Must sum to 1.</summary>
    public const double OpponentSidekickWeight = 0.40;

    public const double OpponentCardWeight = 0.60;

    /// <summary>
    /// Tournament completion bonuses (see <see cref="Tournaments.TournamentAwardScheduler"/>), all
    /// expressed as a multiple of <see cref="KFactor"/> so they stay scale-consistent with match-by-match
    /// rating changes. Every tier doubles the one below it - <see cref="SemifinalistAwardMultiplier"/> is
    /// exactly half of <see cref="FinalistAwardMultiplier"/>, and so on down to
    /// <see cref="SixteenthFinalistAwardMultiplier"/> for the rare 32+ participant bracket.
    /// </summary>
    public const double WinnerAwardMultiplier = 4.0;

    public const double FinalistAwardMultiplier = 2.0;

    public const double SemifinalistAwardMultiplier = 1.0;

    public const double QuarterfinalistAwardMultiplier = 0.5;

    public const double EighthFinalistAwardMultiplier = 0.25;

    public const double SixteenthFinalistAwardMultiplier = 0.125;

    /// <summary>Flat bonus for topping a GroupStage group's round robin, on top of whatever the
    /// participant separately earns from the playoff bracket their group advanced them into.</summary>
    public const double GroupWinnerAwardMultiplier = 1.0;

    /// <summary>League has no bracket, so it uses its own (lower) three-tier scale instead of the
    /// bracket/Swiss one above.</summary>
    public const double LeagueFirstAwardMultiplier = 3.0;

    public const double LeagueSecondAwardMultiplier = 1.5;

    public const double LeagueThirdAwardMultiplier = 0.75;

    /// <summary>The participant count the multipliers above are tuned for. A tournament with more or
    /// fewer participants scales its whole payout schedule by <c>participants / TournamentAwardBaseParticipants</c>,
    /// clamped to <see cref="TournamentAwardScaleMin"/>..<see cref="TournamentAwardScaleMax"/>.</summary>
    public const int TournamentAwardBaseParticipants = 8;

    public const double TournamentAwardScaleMin = 0.5;

    public const double TournamentAwardScaleMax = 1.5;
}
