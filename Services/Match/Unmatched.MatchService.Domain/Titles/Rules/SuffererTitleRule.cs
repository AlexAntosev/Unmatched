namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>Longest loss streak any hero has ever endured - The Streak's mirror image.</summary>
public class SuffererTitleRule(IUnitOfWork unitOfWork) : ITitleRule
{
    public string RuleKey => Titles.Sufferer;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
        => StreakTitleRule.LongestStreakHolderAsync(unitOfWork, wins: false);
}
