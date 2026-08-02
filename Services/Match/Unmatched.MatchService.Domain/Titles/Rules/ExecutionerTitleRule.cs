namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Most sidekick HP destroyed across a career. Only 1v1 matches are attributed - team and FFA
/// matches don't have a single well-defined "opponent" to credit the damage to, so they're skipped
/// rather than guessed at.</summary>
public class ExecutionerTitleRule(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : ITitleRule
{
    public string RuleKey => Titles.Executioner;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public async Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
    {
        var matches = await unitOfWork.Matches.GetFinishedForRatingReplayAsync();

        var totals = new Dictionary<Guid, int>();
        foreach (var oneVOne in matches.Where(m => m.Fighters.Count == 2))
        {
            var fighters = oneVOne.Fighters.ToList();
            var (a, b) = (fighters[0], fighters[1]);
            await CreditAsync(totals, dealer: a.HeroId, target: b);
            await CreditAsync(totals, dealer: b.HeroId, target: a);
        }

        var maxDestroyed = totals.Values.DefaultIfEmpty(0).Max();
        if (maxDestroyed <= 0)
        {
            return new Dictionary<Guid, double?>();
        }

        var holder = totals.Where(kv => kv.Value == maxDestroyed).OrderBy(kv => kv.Key).First().Key;
        return new Dictionary<Guid, double?> { [holder] = maxDestroyed };
    }

    private async Task CreditAsync(Dictionary<Guid, int> totals, Guid dealer, FighterEntity target)
    {
        if (target.SidekickHpLeft is null)
        {
            return;
        }

        var targetHero = await catalogHeroCache.GetAsync(target.HeroId);
        var targetMaxSidekickHp = targetHero!.Sidekicks?.Sum(s => s.Hp * s.Count) ?? 0;
        var destroyed = Math.Max(0, targetMaxSidekickHp - target.SidekickHpLeft.Value);
        if (destroyed <= 0)
        {
            return;
        }

        totals[dealer] = totals.GetValueOrDefault(dealer) + destroyed;
    }
}
