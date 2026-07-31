namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Both teams share one Elo delta computed from their rating means - a team result is a
/// shared result, so every member moves by the same amount regardless of their own rating.
/// <see cref="Validation.TeamVsTeamValidator"/> already guarantees exactly two equal-sized teams,
/// which keeps this strictly zero-sum.</summary>
public class TeamVsTeamRatingCalculator(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : IRatingCalculator
{
    public async Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match)
    {
        var fighters = match.Fighters.ToList();
        var winningTeam = fighters.Where(f => f.IsWinner).ToList();
        var losingTeam = fighters.Where(f => f.IsWinner == false).ToList();

        var ratings = await HeroRatingReader.GetRatingsAsync(unitOfWork, fighters.Select(f => f.HeroId));
        var winningMean = winningTeam.Average(f => ratings[f.HeroId]);
        var losingMean = losingTeam.Average(f => ratings[f.HeroId]);

        var winningResources = await HeroRatingReader.GetResourceStatesAsync(catalogHeroCache, winningTeam);
        var losingResources = await HeroRatingReader.GetResourceStatesAsync(catalogHeroCache, losingTeam);
        var performance = PerformanceModifier.Calculate(winningResources, losingResources);

        var delta = EloRating.Delta((int)Math.Round(winningMean), (int)Math.Round(losingMean), performance);

        var result = new Dictionary<Guid, int>();
        foreach (var fighter in winningTeam)
        {
            result[fighter.HeroId] = delta;
        }

        foreach (var fighter in losingTeam)
        {
            result[fighter.HeroId] = -delta;
        }

        return result;
    }
}
