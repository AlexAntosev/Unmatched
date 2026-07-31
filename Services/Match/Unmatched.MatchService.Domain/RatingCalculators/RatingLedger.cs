namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Applies a set of point deltas to hero ratings, creating a rating row at
/// <see cref="RatingConstants.InitialRating"/> for any hero seen for the first time. Shared by
/// <see cref="MatchHandlers.MatchHandler"/> (match deltas) and tournament completion (award bonuses) -
/// both are just "add these points to these heroes' ratings," and the two must stay identical since a
/// rating replay walks both through the same merged timeline.</summary>
public static class RatingLedger
{
    public static async Task ApplyAsync(IUnitOfWork unitOfWork, IReadOnlyDictionary<Guid, int> pointsByHero)
    {
        foreach (var (heroId, points) in pointsByHero)
        {
            var rating = await unitOfWork.Ratings.GetByHeroIdAsync(heroId)
                ?? new RatingEntity { HeroId = heroId, Points = RatingConstants.InitialRating };

            rating.Points += points;
            await unitOfWork.Ratings.AddOrUpdateAsync(rating);
        }
    }
}
