namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Shared lookups every calculator needs: a hero's current rating (defaulting a
/// never-rated hero to <see cref="RatingConstants.InitialRating"/>) and the resource fractions
/// <see cref="PerformanceModifier"/> scores a fighter on. Extracted so the three calculators don't
/// each re-implement the same two round-trips to the unit of work and the hero cache.</summary>
internal static class HeroRatingReader
{
    public static async Task<int> GetRatingAsync(IUnitOfWork unitOfWork, Guid heroId)
        => (await unitOfWork.Ratings.GetByHeroIdAsync(heroId))?.Points ?? RatingConstants.InitialRating;

    public static async Task<Dictionary<Guid, int>> GetRatingsAsync(IUnitOfWork unitOfWork, IEnumerable<Guid> heroIds)
    {
        var ratings = new Dictionary<Guid, int>();
        foreach (var heroId in heroIds.Distinct())
        {
            ratings[heroId] = await GetRatingAsync(unitOfWork, heroId);
        }

        return ratings;
    }

    public static async Task<List<PerformanceModifier.FighterResourceState>> GetResourceStatesAsync(
        ICatalogHeroCache catalogHeroCache,
        IEnumerable<FighterEntity> fighters)
    {
        var states = new List<PerformanceModifier.FighterResourceState>();
        foreach (var fighter in fighters)
        {
            var hero = await catalogHeroCache.GetAsync(fighter.HeroId);
            states.Add(PerformanceModifier.From(fighter, hero!));
        }

        return states;
    }
}
