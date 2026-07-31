namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Whoever holds the #1 hero rating right now.</summary>
public class GrandChampionTitleRule(IUnitOfWork unitOfWork) : ITitleRule
{
    public string RuleKey => Titles.GrandChampion;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public async Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        var ratings = await unitOfWork.Ratings.GetAsync();
        if (ratings.Count == 0)
        {
            return new HashSet<Guid>();
        }

        var maxPoints = ratings.Max(r => r.Points);
        var champion = ratings.Where(r => r.Points == maxPoints).OrderBy(r => r.HeroId).First().HeroId;
        return new HashSet<Guid> { champion };
    }
}
