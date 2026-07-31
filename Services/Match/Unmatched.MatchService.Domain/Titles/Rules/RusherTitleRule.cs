namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Win with at least two thirds of the deck still unplayed.</summary>
public class RusherTitleRule(ICatalogHeroCache catalogHeroCache) : ITitleRule
{
    public string RuleKey => Titles.Rusher;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public async Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        var qualifiers = new HashSet<Guid>();
        foreach (var winner in match.Fighters.Where(f => f.IsWinner))
        {
            if (winner.CardsLeft is null)
            {
                continue;
            }

            var hero = await catalogHeroCache.GetAsync(winner.HeroId);
            if (winner.CardsLeft >= TitleThresholds.RusherMinCardRatio * hero!.DeckSize)
            {
                qualifiers.Add(winner.HeroId);
            }
        }

        return qualifiers;
    }
}
