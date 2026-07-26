namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Entities;

public class OneVsOneValidator : IGameModeValidator
{
    public void Validate(MatchEntity match)
    {
        if (match.Fighters is null || match.Fighters.Count != 2)
        {
            throw new ArgumentException("A 1v1 match must have exactly 2 fighters.", nameof(match));
        }

        if (match.Fighters.Count(f => f.IsWinner) != 1)
        {
            throw new ArgumentException("A 1v1 match should have one winner.", nameof(match));
        }
    }
}
