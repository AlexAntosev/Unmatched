namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;

/// <summary>One match from a single hero's point of view: the fighter who played it and whoever they faced.</summary>
public record HeroMatchEntry(UiMatchLogDto Match, UiFighterDto Self, IReadOnlyList<MatchParticipant> Opponents);

/// <summary>
/// Reshapes a hero's matches for the "<Name>'s matches" section on the hero page, where - unlike
/// the shared 3b row - results are coloured because the section belongs to one hero. Reuses
/// <see cref="MatchPresentation.Sides"/> so the "opponent" side is correct for every game mode,
/// including co-op where it resolves to the villain.
/// </summary>
public static class HeroMatchPresentation
{
    public static IReadOnlyList<HeroMatchEntry> Build(Guid heroId, IEnumerable<UiMatchLogDto> matches)
    {
        var entries = new List<HeroMatchEntry>();

        foreach (var match in matches)
        {
            var self = match.Fighters.FirstOrDefault(f => f.HeroId == heroId);
            if (self is null)
            {
                continue;
            }

            var sides = MatchPresentation.Sides(match);
            var mySide = sides.FirstOrDefault(side => side.Participants.Any(
                p => p.Kind == ParticipantKind.Hero && p.Id == heroId));

            var opponents = sides
                .Where(side => side != mySide)
                .SelectMany(side => side.Participants)
                .ToList();

            entries.Add(new HeroMatchEntry(match, self, opponents));
        }

        return entries;
    }
}
