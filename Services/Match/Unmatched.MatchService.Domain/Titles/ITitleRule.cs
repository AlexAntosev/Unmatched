namespace Unmatched.MatchService.Domain.Titles;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>
/// One automatic title's qualification logic. <see cref="EvaluateAsync"/> means different things
/// depending on <see cref="Exclusivity"/>: for a <see cref="TitleExclusivity.Shared"/> rule it is "which
/// heroes newly qualify from this match" (a set to add to whoever already holds the title); for a
/// <see cref="TitleExclusivity.Unique"/> rule it is "who holds the record right now, given the whole
/// history including this match" (a set - at most one hero - that becomes the entire holder set,
/// transferring the title away from anyone else). <see cref="TitleEvaluator"/> applies that distinction
/// uniformly so individual rules only need to answer "who qualifies."
///
/// The dictionary's value is the rule-specific "how much/how many" backing the qualification (e.g. total
/// sidekick HP destroyed for Executioner, HP left for Last Breath) - null where no single number is
/// meaningful (e.g. Flawless, Kingslayer). <see cref="TitleEvaluator"/> persists it as-is; only rules
/// where storing it makes sense need to compute anything beyond null.
/// </summary>
public interface ITitleRule
{
    string RuleKey { get; }

    TitleExclusivity Exclusivity { get; }

    Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match);
}
