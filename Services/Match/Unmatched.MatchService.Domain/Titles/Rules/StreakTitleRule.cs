namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Longest win streak any hero has ever put together, walked chronologically over the whole
/// match history (not the old handler's raw Fighters order, which wasn't guaranteed to be chronological
/// and could misjudge streaks).</summary>
public class StreakTitleRule(IUnitOfWork unitOfWork) : ITitleRule
{
    public string RuleKey => Titles.Streak;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public async Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
        => await LongestStreakHolderAsync(unitOfWork, wins: true);

    internal static async Task<IReadOnlyDictionary<Guid, double?>> LongestStreakHolderAsync(IUnitOfWork unitOfWork, bool wins)
    {
        var matches = (await unitOfWork.Matches.GetFinishedForRatingReplayAsync()).OrderBy(m => m.Date);

        var runningStreaks = new Dictionary<Guid, int>();
        var bestStreaks = new Dictionary<Guid, int>();
        foreach (var fighter in matches.SelectMany(m => m.Fighters))
        {
            var qualifies = fighter.IsWinner == wins;
            var current = qualifies ? runningStreaks.GetValueOrDefault(fighter.HeroId) + 1 : 0;
            runningStreaks[fighter.HeroId] = current;
            if (current > bestStreaks.GetValueOrDefault(fighter.HeroId))
            {
                bestStreaks[fighter.HeroId] = current;
            }
        }

        var longest = bestStreaks.Values.DefaultIfEmpty(0).Max();
        if (longest == 0)
        {
            return new Dictionary<Guid, double?>();
        }

        var holder = bestStreaks.Where(kv => kv.Value == longest).OrderBy(kv => kv.Key).Select(kv => kv.Key).First();
        return new Dictionary<Guid, double?> { [holder] = longest };
    }
}
