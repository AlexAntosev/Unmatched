namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Holds the Bounty pool. Always vacant for now - Bounty tournaments have no match generator or
/// completion support yet (see TournamentAwardScheduler/TournamentFormatGeneratorFactory), so there is no
/// pool to hold. The title row exists so the catalogue is complete and ready once Bounty ships.</summary>
public class BountyHolderTitleRule : ITitleRule
{
    public string RuleKey => Titles.BountyHolder;

    public TitleExclusivity Exclusivity => TitleExclusivity.Unique;

    public Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
        => Task.FromResult<IReadOnlyDictionary<Guid, double?>>(new Dictionary<Guid, double?>());
}
