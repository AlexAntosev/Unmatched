namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class OneVsOneRatingCalculator(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : IRatingCalculator
{
    public async Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match)
    {
        var winner = match.Fighters.First(f => f.IsWinner);
        var loser = match.Fighters.First(f => f.IsWinner == false);

        var winnerRating = await HeroRatingReader.GetRatingAsync(unitOfWork, winner.HeroId);
        var loserRating = await HeroRatingReader.GetRatingAsync(unitOfWork, loser.HeroId);

        var winnerHero = await catalogHeroCache.GetAsync(winner.HeroId);
        var loserHero = await catalogHeroCache.GetAsync(loser.HeroId);

        var performance = PerformanceModifier.Calculate(
            winningSide: [PerformanceModifier.From(winner, winnerHero!)],
            losingSide: [PerformanceModifier.From(loser, loserHero!)]);

        var delta = EloRating.Delta(winnerRating, loserRating, performance);

        return new Dictionary<Guid, int>
        {
            [winner.HeroId] = delta,
            [loser.HeroId] = -delta
        };
    }
}
