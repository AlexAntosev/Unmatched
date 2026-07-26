namespace Unmatched.MatchService.Domain.Validation;

using Unmatched.MatchService.Domain.Entities;

public class TeamVsTeamValidator : IGameModeValidator
{
    public void Validate(MatchEntity match)
    {
        if (match.Fighters is null || match.Fighters.Count < 2)
        {
            throw new ArgumentException("A team match must have at least 2 fighters.", nameof(match));
        }

        if (match.Fighters.Any(f => f.Team is null))
        {
            throw new ArgumentException("Every fighter in a team match must be assigned a team.", nameof(match));
        }

        var teams = match.Fighters.GroupBy(f => f.Team!.Value).ToList();
        if (teams.Count != 2)
        {
            throw new ArgumentException("A team match must have exactly 2 teams.", nameof(match));
        }

        if (teams.Select(t => t.Count()).Distinct().Count() != 1)
        {
            throw new ArgumentException("Both teams in a team match must be the same size.", nameof(match));
        }

        if (teams.Any(t => t.Select(f => f.IsWinner).Distinct().Count() != 1))
        {
            throw new ArgumentException("Every fighter within a team must share the same result.", nameof(match));
        }

        if (teams.Count(t => t.First().IsWinner) != 1)
        {
            throw new ArgumentException("A team match should have exactly one winning team.", nameof(match));
        }
    }
}
