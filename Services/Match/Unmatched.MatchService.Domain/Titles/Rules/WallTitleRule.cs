namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Lowest average HP lost per win, among heroes with at least one win where HP was recorded.</summary>
public class WallTitleRule(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : ITitleRule
{
    public string RuleKey => Titles.Wall;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public async Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
    {
        var matches = await unitOfWork.Matches.GetFinishedForRatingReplayAsync();

        var hpLostByHero = new Dictionary<Guid, int>();
        var winCountByHero = new Dictionary<Guid, int>();
        foreach (var fighter in matches.SelectMany(m => m.Fighters).Where(f => f.IsWinner && f.HpLeft.HasValue))
        {
            var hero = await catalogHeroCache.GetAsync(fighter.HeroId);
            var lost = Math.Max(0, hero!.Hp - fighter.HpLeft!.Value);
            hpLostByHero[fighter.HeroId] = hpLostByHero.GetValueOrDefault(fighter.HeroId) + lost;
            winCountByHero[fighter.HeroId] = winCountByHero.GetValueOrDefault(fighter.HeroId) + 1;
        }

        if (winCountByHero.Count == 0)
        {
            return new Dictionary<Guid, double?>();
        }

        var lowestAverage = winCountByHero.Keys
            .Select(heroId => (heroId, average: (double)hpLostByHero[heroId] / winCountByHero[heroId]))
            .OrderBy(x => x.average)
            .ThenBy(x => x.heroId)
            .First();

        return new Dictionary<Guid, double?> { [lowestAverage.heroId] = lowestAverage.average };
    }
}
