namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Win with at most <see cref="TitleThresholds.LastBreathMaxHp"/> HP left - a squeaker.</summary>
public class LastBreathTitleRule : ITitleRule
{
    public string RuleKey => Titles.LastBreath;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        IReadOnlySet<Guid> qualifiers = match.Fighters
            .Where(f => f.IsWinner && f.HpLeft is >= 0 and <= TitleThresholds.LastBreathMaxHp)
            .Select(f => f.HeroId)
            .ToHashSet();

        return Task.FromResult(qualifiers);
    }
}
