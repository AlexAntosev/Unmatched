namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Entities;

public class CooperativeValidator : IGameModeValidator
{
    public void Validate(MatchEntity match)
    {
        if (match.Fighters is null || match.Fighters.Count < 1)
        {
            throw new ArgumentException("A cooperative match must have at least 1 fighter.", nameof(match));
        }

        if (match.Villain is null)
        {
            throw new ArgumentException("A cooperative match must have a Villain.", nameof(match));
        }

        if (match.Fighters.Select(f => f.IsWinner).Distinct().Count() != 1)
        {
            throw new ArgumentException("Every player in a cooperative match must share the same result.", nameof(match));
        }

        var playersWon = match.Fighters.First().IsWinner;
        if (match.Villain.IsWinner == playersWon)
        {
            throw new ArgumentException("The Villain must win exactly when the players don't.", nameof(match));
        }

        if (match.Villain.Minions.Any(m => m.IsWinner != match.Villain.IsWinner))
        {
            throw new ArgumentException("Every minion must share the Villain's result.", nameof(match));
        }
    }
}
