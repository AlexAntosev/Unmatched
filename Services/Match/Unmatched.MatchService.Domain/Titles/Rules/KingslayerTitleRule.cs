namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>The last hero to beat the current #1-rated hero. Vacant if the reigning champion has never
/// lost a match.</summary>
public class KingslayerTitleRule(IUnitOfWork unitOfWork) : ITitleRule
{
    public string RuleKey => Titles.Kingslayer;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public async Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        var ratings = await unitOfWork.Ratings.GetAsync();
        if (ratings.Count == 0)
        {
            return new HashSet<Guid>();
        }

        var maxPoints = ratings.Max(r => r.Points);
        var championId = ratings.Where(r => r.Points == maxPoints).OrderBy(r => r.HeroId).First().HeroId;

        var matches = await unitOfWork.Matches.GetFinishedForRatingReplayAsync();
        var lastLoss = matches
            .Where(m => m.Fighters.Any(f => f.HeroId == championId && !f.IsWinner))
            .OrderByDescending(m => m.Date)
            .FirstOrDefault();

        var slayer = lastLoss?.Fighters.FirstOrDefault(f => f.HeroId != championId && f.IsWinner);
        return slayer is null ? new HashSet<Guid>() : new HashSet<Guid> { slayer.HeroId };
    }
}
