namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Most ranked matches played, career-wide.</summary>
public class WorkhorseTitleRule(IUnitOfWork unitOfWork) : ITitleRule
{
    public string RuleKey => Titles.Workhorse;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public async Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        var matches = await unitOfWork.Matches.GetFinishedForRatingReplayAsync();

        var counts = matches
            .Where(m => m.IsRanked)
            .SelectMany(m => m.Fighters)
            .GroupBy(f => f.HeroId)
            .ToDictionary(g => g.Key, g => g.Count());

        if (counts.Count == 0)
        {
            return new HashSet<Guid>();
        }

        var maxMatches = counts.Values.Max();
        var holder = counts.Where(kv => kv.Value == maxMatches).OrderBy(kv => kv.Key).First().Key;
        return new HashSet<Guid> { holder };
    }
}
