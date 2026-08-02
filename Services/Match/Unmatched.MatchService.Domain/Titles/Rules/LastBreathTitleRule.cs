namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Win with at most <see cref="TitleThresholds.LastBreathMaxHp"/> HP left - a squeaker.</summary>
public class LastBreathTitleRule : ITitleRule
{
    public string RuleKey => Titles.LastBreath;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
    {
        var qualifiers = new Dictionary<Guid, double?>();
        foreach (var f in match.Fighters.Where(f => f.IsWinner && f.HpLeft is >= 0 and <= TitleThresholds.LastBreathMaxHp))
        {
            qualifiers[f.HeroId] = f.HpLeft;
        }

        return Task.FromResult<IReadOnlyDictionary<Guid, double?>>(qualifiers);
    }
}
