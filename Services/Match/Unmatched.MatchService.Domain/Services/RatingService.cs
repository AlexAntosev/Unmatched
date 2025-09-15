namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Repositories;

public class RatingService(IMatchService matchService, IUnitOfWork unitOfWork, IMapper mapper) : IRatingService
{
    public async Task<List<RatingChange>> GetRatingChangesAsync(Guid heroId)
    {
        var ratingChanges = new List<RatingChange>();
        var currentRating = await unitOfWork.Ratings.GetByHeroIdAsync(heroId);
        ratingChanges.Add(
            new RatingChange
                {
                    Date = "Current",
                    RatingDelta = currentRating?.Points ?? 0
                });

        var points = currentRating?.Points ?? 0;

        var heroMatches = await unitOfWork.Matches.GetFinishedByHeroIdAsync(heroId);

        foreach (var heroMatch in heroMatches)
        {
            var matchPoints = heroMatch.Fighters.FirstOrDefault(f => f.HeroId == heroId)?.MatchPoints ?? 0;
            points -= matchPoints;
            ratingChanges.Add(
                new RatingChange
                    {
                        Date = heroMatch.Date.ToShortDateString(),
                        RatingDelta = points
                    });
        }

        ratingChanges.Reverse();

        return ratingChanges;
    }

    public async Task RecalculateAsync()
    {
        var matches = await GetMatchesAsync();

        await ClearDataAsync();

        foreach (var match in matches)
        {
            await matchService.AddOrUpdateAsync(match);
        }
    }

    private async Task ClearDataAsync()
    {
        unitOfWork.Matches.DeleteAll();
        unitOfWork.Fighters.DeleteAll();
        unitOfWork.Ratings.DeleteAll();

        await unitOfWork.SaveChangesAsync();
    }

    private async Task<IEnumerable<Match>> GetMatchesAsync()
    {
        var matches = await unitOfWork.Matches.GetAsync();
        return matches.Select(mapper.Map<Match>);
    }
}