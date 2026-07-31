namespace Unmatched.MatchService.Domain.Services;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>One event in a hero's rating history - either a finished match or a tournament completion
/// award. Exactly one of <see cref="Match"/>/<see cref="Award"/> is set.</summary>
public record RatingEvent(DateTime OccurredAt, MatchEntity? Match, TournamentAwardEntity? Award);

/// <summary>
/// Merges every finished match and every tournament award into one date-ordered stream - the full
/// history of everything that has ever moved a hero's rating. Elo's expected score depends on the
/// rating *at that instant*, so a mid-history award changes every later match's delta: a rebase
/// (<see cref="IRatingService.RecalculateAsync"/>) and the rating history chart
/// (<see cref="IRatingService.GetRatingChangesAsync"/>) must walk this exact same merged order rather
/// than compute matches and awards as two separate passes.
/// </summary>
public class RatingTimeline(IUnitOfWork unitOfWork)
{
    public async Task<List<RatingEvent>> BuildAsync()
    {
        var matches = await unitOfWork.Matches.GetFinishedForRatingReplayAsync();
        var awards = await unitOfWork.TournamentAwards.GetAsync();

        return matches.Select(m => new RatingEvent(m.Date, m, null))
            .Concat(awards.Select(a => new RatingEvent(a.AwardedAt, null, a)))
            .OrderBy(e => e.OccurredAt)
            .ToList();
    }
}
