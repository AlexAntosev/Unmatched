namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Decomposes a free-for-all result into every pairwise "mini-duel" implied by the final
/// placements, and sums each fighter's pairwise Elo surprises into their delta.
/// <see cref="Validation.FreeForAllValidator"/> already guarantees placements are a 1..N
/// permutation, so there are no ties to resolve. The margin-of-victory modifier is judged on the
/// 1st-place fighter alone - free-for-all has no single "loser side" to compare against.</summary>
public class FreeForAllRatingCalculator(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : IRatingCalculator
{
    public async Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match)
    {
        var fighters = match.Fighters.ToList();
        var ratings = await HeroRatingReader.GetRatingsAsync(unitOfWork, fighters.Select(f => f.HeroId));

        var winner = fighters.First(f => f.Placement == 1);
        var winnerHero = await catalogHeroCache.GetAsync(winner.HeroId);
        var performance = PerformanceModifier.Calculate(
            winningSide: [PerformanceModifier.From(winner, winnerHero!)],
            losingSide: []);

        var result = new Dictionary<Guid, int>();
        var opponentCount = fighters.Count - 1;

        foreach (var fighter in fighters)
        {
            var pairwiseSurprise = 0.0;
            foreach (var opponent in fighters)
            {
                if (opponent == fighter)
                {
                    continue;
                }

                var expected = EloRating.ExpectedScore(ratings[fighter.HeroId], ratings[opponent.HeroId]);
                var actual = fighter.Placement < opponent.Placement ? 1.0 : 0.0;
                pairwiseSurprise += actual - expected;
            }

            result[fighter.HeroId] = (int)Math.Round(
                RatingConstants.KFactor / (double)opponentCount * performance * pairwiseSurprise,
                MidpointRounding.AwayFromZero);
        }

        return result;
    }
}
