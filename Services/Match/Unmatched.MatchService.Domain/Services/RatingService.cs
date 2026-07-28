namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Repositories;

public class RatingService(IMatchHandlerFactory matchHandlerFactory, IUnitOfWork unitOfWork, IMapper mapper) : IRatingService
{
    public async Task<IEnumerable<Rating>> GetAllAsync()
    {
        var ratingEntity = await unitOfWork.Ratings.GetAsync();
        return mapper.Map<IEnumerable<Rating>>(ratingEntity);
    }

    public async Task<Rating> GetByHeroAsync(Guid heroId)
    {
        var ratingEntity = await unitOfWork.Ratings.GetByHeroIdAsync(heroId);
        return mapper.Map<Rating>(ratingEntity);
    }

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

    public Task<bool> IsRecalculationRequiredAsync()
        => unitOfWork.RatingRecalculationState.IsRecalculationRequiredAsync();

    /// <remarks>
    /// Ratings and Fighters.MatchPoints are values derived from the match history, so a recalculation only
    /// resets those and replays the history over them - the matches and fighters themselves are never
    /// deleted. Replaying goes straight through the match handlers rather than through IMatchService so
    /// that re-deriving old ratings doesn't re-publish a match-created event per match (which would
    /// double-count every match in the statistics service) or re-award titles.
    /// </remarks>
    public async Task RecalculateAsync()
    {
        var matches = (await unitOfWork.Matches.GetFinishedForRatingReplayAsync()).OrderBy(m => m.Date).ToList();

        // if the replay dies halfway through, the ratings left behind are derived from only part of the
        // history - keep the flag raised until it has fully succeeded so the UI keeps asking for a re-run.
        await unitOfWork.RatingRecalculationState.SetRecalculationRequiredAsync(true);

        unitOfWork.Ratings.DeleteAll();
        await unitOfWork.SaveChangesAsync();

        foreach (var match in matches)
        {
            var handler = matchHandlerFactory.Create(match);
            await handler.HandleAsync(match);
        }

        await unitOfWork.RatingRecalculationState.SetRecalculationRequiredAsync(false);
    }
}
