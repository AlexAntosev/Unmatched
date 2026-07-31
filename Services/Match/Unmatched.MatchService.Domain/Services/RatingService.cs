namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

public class RatingService(
    IMatchHandler matchHandler, IUnitOfWork unitOfWork, IMapper mapper, RatingTimeline ratingTimeline) : IRatingService
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

    /// <remarks>
    /// Walked forward from <see cref="RatingConstants.InitialRating"/> over the same merged
    /// matches+awards timeline a recalculation replays - not backward from the current total, which
    /// would drift by the hero's award total the moment tournament bonuses exist.
    /// </remarks>
    public async Task<List<RatingChange>> GetRatingChangesAsync(Guid heroId)
    {
        var heroEvents = (await ratingTimeline.BuildAsync()).Where(e => Involves(e, heroId)).ToList();

        var ratingChanges = new List<RatingChange>();
        var points = RatingConstants.InitialRating;
        foreach (var ratingEvent in heroEvents)
        {
            points += DeltaFor(ratingEvent, heroId);
            ratingChanges.Add(new RatingChange { Date = ratingEvent.OccurredAt.ToShortDateString(), RatingDelta = points });
        }

        return ratingChanges;
    }

    public Task<bool> IsRecalculationRequiredAsync()
        => unitOfWork.RatingRecalculationState.IsRecalculationRequiredAsync();

    /// <remarks>
    /// Ratings and Fighters.MatchPoints are values derived from the match history, so a recalculation only
    /// resets those and replays the history over them - the matches, fighters and awards themselves are
    /// never deleted. Replaying matches goes straight through the match handler rather than through
    /// IMatchService so that re-deriving old ratings doesn't re-publish a match-created event per match
    /// (which would double-count every match in the statistics service) or re-award titles.
    /// </remarks>
    public async Task RecalculateAsync()
    {
        var timeline = await ratingTimeline.BuildAsync();

        // if the replay dies halfway through, the ratings left behind are derived from only part of the
        // history - keep the flag raised until it has fully succeeded so the UI keeps asking for a re-run.
        await unitOfWork.RatingRecalculationState.SetRecalculationRequiredAsync(true);

        unitOfWork.Ratings.DeleteAll();
        await unitOfWork.SaveChangesAsync();

        foreach (var ratingEvent in timeline)
        {
            if (ratingEvent.Match is not null)
            {
                await matchHandler.HandleAsync(ratingEvent.Match);
            }
            else if (ratingEvent.Award is not null)
            {
                await RatingLedger.ApplyAsync(unitOfWork, new Dictionary<Guid, int> { [ratingEvent.Award.HeroId] = ratingEvent.Award.Points });
                await unitOfWork.SaveChangesAsync();
            }
        }

        await unitOfWork.RatingRecalculationState.SetRecalculationRequiredAsync(false);
    }

    private static bool Involves(RatingEvent ratingEvent, Guid heroId)
        => ratingEvent.Match?.Fighters.Any(f => f.HeroId == heroId) == true || ratingEvent.Award?.HeroId == heroId;

    private static int DeltaFor(RatingEvent ratingEvent, Guid heroId)
        => ratingEvent.Match is not null
            ? ratingEvent.Match.Fighters.First(f => f.HeroId == heroId).MatchPoints ?? 0
            : ratingEvent.Award!.Points;
}
