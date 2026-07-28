namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Entities;

public class FreeForAllValidator : IGameModeValidator
{
    public void Validate(MatchEntity match)
    {
        if (match.Fighters is null || match.Fighters.Count < 3)
        {
            throw new ArgumentException("A free-for-all match must have at least 3 fighters.", nameof(match));
        }

        if (match.Fighters.Any(f => f.Placement is null))
        {
            throw new ArgumentException("Every fighter in a free-for-all match must be assigned a placement.", nameof(match));
        }

        var placements = match.Fighters.Select(f => f.Placement!.Value).OrderBy(p => p).ToList();
        var expectedPlacements = Enumerable.Range(1, match.Fighters.Count);
        if (!placements.SequenceEqual(expectedPlacements))
        {
            throw new ArgumentException("Placements in a free-for-all match must be a permutation of 1..N.", nameof(match));
        }

        if (match.Fighters.Any(f => f.IsWinner != (f.Placement == 1)))
        {
            throw new ArgumentException("Only the fighter placed 1st should be marked as the winner.", nameof(match));
        }
    }
}
