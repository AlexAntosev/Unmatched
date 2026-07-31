namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Win a single ranked match with a rating gain of at least <see cref="TitleThresholds.PunisherMinMatchPoints"/>
/// - an underdog beating a much stronger opponent. Retuned from the old "MatchPoints >= 1000", which was
/// unreachable once rating moved to Elo's K=32 scale.</summary>
public class PunisherTitleRule : ITitleRule
{
    public string RuleKey => Titles.Punisher;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        IReadOnlySet<Guid> qualifiers = match.Fighters
            .Where(f => f.IsWinner && (f.MatchPoints ?? 0) >= TitleThresholds.PunisherMinMatchPoints)
            .Select(f => f.HeroId)
            .ToHashSet();

        return Task.FromResult(qualifiers);
    }
}
