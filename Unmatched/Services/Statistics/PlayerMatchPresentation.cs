namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

/// <summary>One match from a single player's point of view: their fighter and whoever they faced.</summary>
public record PlayerMatchEntry(UiMatchLogDto Match, UiFighterDto Self, IReadOnlyList<MatchParticipant> Opponents);

/// <summary>
/// Reshapes a player's matches for the "<Name>'s matches" section on the player page, where - unlike
/// the shared 3b row - results are coloured because the section belongs to one person. Reuses
/// <see cref="MatchPresentation.Sides"/> so the "opponent" side is correct for every game mode,
/// including co-op where it resolves to the villain.
/// </summary>
public static class PlayerMatchPresentation
{
    public static IReadOnlyList<PlayerMatchEntry> Build(Guid playerId, IEnumerable<UiMatchLogDto> matches)
    {
        var entries = new List<PlayerMatchEntry>();

        foreach (var match in matches)
        {
            var self = match.Fighters.FirstOrDefault(f => f.PlayerId == playerId);
            if (self is null)
            {
                continue;
            }

            var sides = MatchPresentation.Sides(match);
            var mySide = sides.FirstOrDefault(side => side.Participants.Any(
                p => p.Kind == ParticipantKind.Hero && p.Name == self.Hero?.Name && p.PlayerName == self.Player?.Name));

            var opponents = sides
                .Where(side => side != mySide)
                .SelectMany(side => side.Participants)
                .ToList();

            entries.Add(new PlayerMatchEntry(match, self, opponents));
        }

        return entries;
    }
}
