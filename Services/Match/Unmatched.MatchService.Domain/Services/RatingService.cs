namespace Unmatched.MatchService.Domain.Services;

using AutoMapper;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Tournaments;

public class RatingService(
    IMatchHandler matchHandler,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    RatingTimeline ratingTimeline,
    TitleEvaluator titleEvaluator,
    TournamentTitleAwarder titleAwarder,
    BountyChallengeResolver bountyChallengeResolver) : IRatingService
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
    /// Ratings, Fighters.MatchPoints and HeroTitles are values derived from the match/award history, so a
    /// recalculation only resets those and replays the history over them - the matches, fighters, awards
    /// and tournaments themselves are never deleted. Replaying matches goes straight through the match
    /// handler rather than through IMatchService so that re-deriving old ratings doesn't re-publish a
    /// match-created event per match (which would double-count every match in the statistics service).
    /// Titles ARE re-evaluated inline on the same walk, though: rules like GrandChampion/Kingslayer/
    /// Cinderella read the live rating table, so they need to run at each event's position in the replay
    /// (the same reason ratings themselves can't be computed out of order), not once at the end.
    /// </remarks>
    public async Task RecalculateAsync()
    {
        var timeline = await ratingTimeline.BuildAsync();

        // if the replay dies halfway through, the ratings left behind are derived from only part of the
        // history - keep the flag raised until it has fully succeeded so the UI keeps asking for a re-run.
        await unitOfWork.RatingRecalculationState.SetRecalculationRequiredAsync(true);

        unitOfWork.Ratings.DeleteAll();
        unitOfWork.HeroTitles.DeleteAll();
        await unitOfWork.SaveChangesAsync();

        var titledTournaments = new HashSet<Guid>();

        foreach (var ratingEvent in timeline)
        {
            if (ratingEvent.Match is not null)
            {
                await matchHandler.HandleAsync(ratingEvent.Match);
                if (ratingEvent.Match.GameMode != GameMode.Cooperative)
                {
                    await titleEvaluator.EvaluateAsync(ratingEvent.Match);
                }
            }
            else if (ratingEvent.Award is not null)
            {
                if (ratingEvent.Award.AwardKind == TournamentAwardKind.BountyChallengeWin)
                {
                    // Bounty awards land one per challenge, each with its own date - unlike every other
                    // kind, which arrive in one same-dated batch at completion, so each replays on its
                    // own rather than being grouped per tournament like ReplayTournamentCompletionAsync.
                    await bountyChallengeResolver.ReplayAsync(ratingEvent.Award);
                }
                else if (titledTournaments.Add(ratingEvent.Award.TournamentId))
                {
                    await ReplayTournamentCompletionAsync(ratingEvent.Award.TournamentId);
                }
            }
        }

        await unitOfWork.RatingRecalculationState.SetRecalculationRequiredAsync(false);
    }

    /// <summary>Applies every award of a completed tournament and re-runs its title awarder together, the
    /// first time any one of its awards is reached in the replay - awards are grouped rather than applied
    /// one event at a time so Cinderella (which reads every participant's live rating) sees the same
    /// fully-applied tournament payout <see cref="TournamentService.CompleteAsync"/> produced.</summary>
    private async Task ReplayTournamentCompletionAsync(Guid tournamentId)
    {
        var awards = await unitOfWork.TournamentAwards.GetByTournamentAsync(tournamentId);
        var pointsByHero = awards.GroupBy(a => a.HeroId).ToDictionary(g => g.Key, g => g.Sum(a => a.Points));
        await RatingLedger.ApplyAsync(unitOfWork, pointsByHero);

        var tournament = await unitOfWork.Tournaments.GetByIdWithParticipantsAsync(tournamentId);
        var tournamentMatches = await unitOfWork.Matches.GetByTournamentAsync(tournamentId);
        await titleAwarder.AwardAsync(tournament!, tournamentMatches);

        await unitOfWork.SaveChangesAsync();
    }

    private static bool Involves(RatingEvent ratingEvent, Guid heroId)
        => ratingEvent.Match?.Fighters.Any(f => f.HeroId == heroId) == true || ratingEvent.Award?.HeroId == heroId;

    private static int DeltaFor(RatingEvent ratingEvent, Guid heroId)
        => ratingEvent.Match is not null
            ? ratingEvent.Match.Fighters.First(f => f.HeroId == heroId).MatchPoints ?? 0
            : ratingEvent.Award!.Points;
}
