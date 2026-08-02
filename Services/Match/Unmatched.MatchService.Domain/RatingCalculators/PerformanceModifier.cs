namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;

/// <summary>
/// Margin-of-victory multiplier on top of the base Elo delta: how much of its own HP/sidekick
/// HP/cards the winning side kept (more is better), and how much the losing side kept (less is
/// better - see design notes below for why the losing side's HP is excluded).
/// </summary>
/// <remarks>
/// Symmetric by design: a side is scored on its own remaining resources when it won, and on how
/// thoroughly it was defeated when it lost. A side's HP left is only meaningful on the winning side -
/// reducing the opponent's HP to 0 is the win condition, so the losing side's HP is always 0 and
/// carries no information; its weight is folded into sidekicks and cards instead. A hero with no
/// sidekicks has that weight folded proportionally into HP and cards, so such heroes are neither
/// penalised nor rewarded for something they don't have. Any side with missing match data (HP/cards
/// not recorded, or sidekick HP not recorded for a hero that has sidekicks) scores a neutral 0.5
/// rather than throwing - this is what lets <c>RatingService.RecalculateAsync</c> safely replay
/// historical matches that predate ranked-data validation.
/// </remarks>
public static class PerformanceModifier
{
    private const double Neutral = 0.5;

    public readonly record struct FighterResourceState(
        int? HpLeft,
        int? SidekickHpLeft,
        int? CardsLeft,
        int HeroMaxHp,
        int HeroMaxSidekickHp,
        int HeroDeckSize);

    public static FighterResourceState From(FighterEntity fighter, CatalogHeroDto hero)
        => new(
            fighter.HpLeft,
            fighter.SidekickHpLeft,
            fighter.CardsLeft,
            hero.Hp,
            hero.Sidekicks?.Sum(s => s.Hp * s.Count) ?? 0,
            hero.DeckSize);

    /// <summary>Combines a winning side and a losing side into one multiplier in
    /// [PerformanceModifierBase, PerformanceModifierBase + PerformanceModifierRange]. Either side may
    /// be empty when there is no meaningful "opponent" to compare against (e.g. free-for-all margin
    /// is judged on the winner alone) - an empty side scores neutral, the same as missing data.</summary>
    public static double Calculate(IReadOnlyCollection<FighterResourceState> winningSide, IReadOnlyCollection<FighterResourceState> losingSide)
    {
        var ownScore = winningSide.Count == 0 ? Neutral : winningSide.Average(OwnScore);
        var oppScore = losingSide.Count == 0 ? Neutral : losingSide.Average(OppScore);
        var dominance = 0.5 * ownScore + 0.5 * (1 - oppScore);

        return RatingConstants.PerformanceModifierBase + RatingConstants.PerformanceModifierRange * dominance;
    }

    private static double OwnScore(FighterResourceState state)
    {
        if (state.HpLeft is null || state.CardsLeft is null)
        {
            return Neutral;
        }

        var hpFrac = Clamp01(state.HpLeft.Value, state.HeroMaxHp);
        var cardFrac = Clamp01(state.CardsLeft.Value, state.HeroDeckSize);

        if (state.HeroMaxSidekickHp <= 0)
        {
            var (hpWeight, cardWeight) = RedistributeSidekickWeight(
                RatingConstants.OwnHpWeight,
                RatingConstants.OwnCardWeight,
                RatingConstants.OwnSidekickWeight);
            return hpWeight * hpFrac + cardWeight * cardFrac;
        }

        if (state.SidekickHpLeft is null)
        {
            return Neutral;
        }

        var sidekickFrac = Clamp01(state.SidekickHpLeft.Value, state.HeroMaxSidekickHp);
        return RatingConstants.OwnHpWeight * hpFrac
            + RatingConstants.OwnSidekickWeight * sidekickFrac
            + RatingConstants.OwnCardWeight * cardFrac;
    }

    private static double OppScore(FighterResourceState state)
    {
        if (state.CardsLeft is null)
        {
            return Neutral;
        }

        var cardFrac = Clamp01(state.CardsLeft.Value, state.HeroDeckSize);

        if (state.HeroMaxSidekickHp <= 0)
        {
            return cardFrac;
        }

        if (state.SidekickHpLeft is null)
        {
            return Neutral;
        }

        var sidekickFrac = Clamp01(state.SidekickHpLeft.Value, state.HeroMaxSidekickHp);
        return RatingConstants.OpponentSidekickWeight * sidekickFrac + RatingConstants.OpponentCardWeight * cardFrac;
    }

    /// <summary>Spreads a dropped weight (e.g. "no sidekicks to score") across the remaining weights
    /// in proportion to their existing share, so the remaining weights still sum to 1.</summary>
    private static (double First, double Second) RedistributeSidekickWeight(double firstWeight, double secondWeight, double droppedWeight)
    {
        var remaining = firstWeight + secondWeight;
        return (
            firstWeight + droppedWeight * (firstWeight / remaining),
            secondWeight + droppedWeight * (secondWeight / remaining));
    }

    private static double Clamp01(int value, int max)
        => max <= 0 ? 0 : Math.Clamp((double)value / max, 0, 1);
}
