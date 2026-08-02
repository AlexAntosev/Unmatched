namespace Unmatched.MatchService.Domain.Titles.Rules;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>Win with zero cards left in the deck.</summary>
public class DeckMillerTitleRule : ITitleRule
{
    public string RuleKey => Titles.DeckMiller;

    public TitleExclusivity Exclusivity => TitleExclusivity.Shared;

    public Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
    {
        // No metric worth storing: qualifying is an exact-zero condition, not a varying figure.
        var qualifiers = new Dictionary<Guid, double?>();
        foreach (var f in match.Fighters.Where(f => f.IsWinner && f.CardsLeft == 0))
        {
            qualifiers[f.HeroId] = null;
        }

        return Task.FromResult<IReadOnlyDictionary<Guid, double?>>(qualifiers);
    }
}
