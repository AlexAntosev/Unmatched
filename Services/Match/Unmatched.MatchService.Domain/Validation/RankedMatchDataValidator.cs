namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>
/// Ranked scoring needs real HP/sidekick/card data - without it, the historical calculator used to
/// crash on null rather than reject the input. This is an input-shape rule, not a domain invariant,
/// so it deliberately does <b>not</b> join the <see cref="IGameModeValidator"/> chain that
/// <see cref="MatchHandlers.MatchHandler"/> runs: <c>RatingService.RecalculateAsync</c> replays every
/// historical match through the handler, and a chained validator here would start rejecting matches
/// that predate this rule. Instead it is called once, from <c>MatchService.AddOrUpdateAsync</c>,
/// only on the save path a real user goes through - never on replay.
/// </summary>
public class RankedMatchDataValidator(ICatalogHeroCache catalogHeroCache)
{
    public async Task ValidateAsync(MatchEntity match)
    {
        // Co-op never affects rating (see NoRatingCalculator) and its resource data lives on the
        // Villain/Minion entities, not on FighterEntity, so none of this applies to it.
        if (match.GameMode == GameMode.Cooperative)
        {
            return;
        }

        var requiresCompleteData = match.IsRanked || match.TournamentId is not null;
        if (!requiresCompleteData)
        {
            return;
        }

        foreach (var fighter in match.Fighters)
        {
            if (fighter.HpLeft is null || fighter.CardsLeft is null)
            {
                throw new ArgumentException(
                    $"Ranked matches require HP left and cards left for every fighter - hero {fighter.HeroId} is missing one.",
                    nameof(match));
            }

            var hero = await catalogHeroCache.GetAsync(fighter.HeroId);
            var heroHasSidekicks = hero?.Sidekicks?.Any(s => s.Count > 0) ?? false;
            if (heroHasSidekicks && fighter.SidekickHpLeft is null)
            {
                throw new ArgumentException(
                    $"Ranked matches require sidekick HP left for a fighter whose hero has a sidekick - hero {fighter.HeroId} is missing it.",
                    nameof(match));
            }
        }
    }
}
