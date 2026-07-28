namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Entities;

public interface ITeamVsTeamRatingCalculator
{
    Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match);
}
