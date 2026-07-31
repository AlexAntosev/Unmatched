namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Win with zero cards left in the deck.</summary>
public class DeckMillerTitleRule : ITitleRule
{
    public string RuleKey => Titles.DeckMiller;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public Task<IReadOnlySet<Guid>> EvaluateAsync(MatchEntity match)
    {
        IReadOnlySet<Guid> qualifiers = match.Fighters
            .Where(f => f.IsWinner && f.CardsLeft == 0)
            .Select(f => f.HeroId)
            .ToHashSet();

        return Task.FromResult(qualifiers);
    }
}
