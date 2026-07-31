namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Win without losing a single point of HP.</summary>
public class FlawlessTitleRule(ICatalogHeroCache catalogHeroCache) : ITitleRule
{
    public string RuleKey => Titles.Flawless;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public async Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        var qualifiers = new HashSet<Guid>();
        foreach (var winner in match.Fighters.Where(f => f.IsWinner))
        {
            if (winner.HpLeft is null)
            {
                continue;
            }

            var hero = await catalogHeroCache.GetAsync(winner.HeroId);
            if (winner.HpLeft >= hero!.Hp)
            {
                qualifiers.Add(winner.HeroId);
            }
        }

        return qualifiers;
    }
}
