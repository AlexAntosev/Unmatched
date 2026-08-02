namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.Enums;

public enum ParticipantKind
{
    Hero,
    Villain,
    Minion
}

/// <summary>
/// One entry in a match, flattened so heroes, villains and minions can be laid out by the same
/// component. Villains and minions carry no player - they are the game, not a person.
/// </summary>
public record MatchParticipant(
    ParticipantKind Kind,
    Guid Id,
    string Name,
    string ImageUrl,
    bool IsWinner,
    string? PlayerName = null,
    string? PlayerImageUrl = null,
    int? Place = null,
    int? HpLeft = null,
    int? CardsLeft = null,
    string? SidekickName = null,
    int? SidekickHp = null,
    int? RatingDelta = null);

/// <summary>
/// A group of participants that faced the others together. <paramref name="Separator"/> is what
/// precedes the group - "VS" between two sides, "›" between free-for-all placings, null for the
/// first. <paramref name="Label"/> is set only where the match actually has named sides.
/// </summary>
public record MatchSide(string? Separator, IReadOnlyList<MatchParticipant> Participants, bool IsWinner, string? Label = null);

/// <summary>One row of the expanded scoreboard.</summary>
public record ScoreboardEntry(MatchParticipant Participant, string? SideLabel, bool IsIndented);

/// <summary>
/// Reshapes a match into the sides the collapsed row shows and the flat list the expanded
/// scoreboard shows. The game mode decides both, which is exactly what the design means by
/// "let the game mode reshape the row".
/// </summary>
public static class MatchPresentation
{
    private const string Versus = "VS";
    private const string Then = "›";

    public static IReadOnlyList<MatchSide> Sides(UiMatchLogDto match)
        => match.GameMode switch
        {
            GameMode.TeamVsTeam => TeamSides(match),
            GameMode.FreeForAll => FreeForAllSides(match),
            GameMode.Cooperative => CooperativeSides(match),
            _ => DuelSides(match)
        };

    /// <summary>
    /// Every participant, in reading order. The Side column is only meaningful where the sides have
    /// names, so it stays null for one-on-one and free-for-all.
    /// </summary>
    public static IReadOnlyList<ScoreboardEntry> Scoreboard(UiMatchLogDto match)
    {
        var entries = new List<ScoreboardEntry>();

        foreach (var side in Sides(match))
        {
            foreach (var participant in side.Participants)
            {
                entries.Add(new ScoreboardEntry(participant, side.Label, IsIndented: false));

                // Minions belong to their villain, so they follow it indented rather than forming
                // a side of their own.
                if (participant.Kind != ParticipantKind.Villain || match.Villain is null)
                {
                    continue;
                }

                foreach (var minion in match.Villain.Minions)
                {
                    entries.Add(new ScoreboardEntry(ToParticipant(minion), side.Label, IsIndented: true));
                }
            }
        }

        return entries;
    }

    private static IReadOnlyList<MatchSide> DuelSides(UiMatchLogDto match)
        => match.Fighters
            .OrderBy(f => f.Turn ?? int.MaxValue)
            .Select((fighter, index) => new MatchSide(
                index == 0 ? null : Versus,
                [ToParticipant(fighter)],
                fighter.IsWinner))
            .ToList();

    private static IReadOnlyList<MatchSide> TeamSides(UiMatchLogDto match)
        => match.Fighters
            .GroupBy(f => f.Team ?? 1)
            .OrderBy(group => group.Key)
            .Select((group, index) => new MatchSide(
                index == 0 ? null : Versus,
                group.Select(ToParticipant).ToList(),
                group.Any(f => f.IsWinner),
                $"Team {group.Key}"))
            .ToList();

    private static IReadOnlyList<MatchSide> FreeForAllSides(UiMatchLogDto match)
        => match.Fighters
            .OrderBy(f => f.Placement ?? int.MaxValue)
            .Select((fighter, index) => new MatchSide(
                index == 0 ? null : Then,
                [ToParticipant(fighter)],
                fighter.IsWinner))
            .ToList();

    private static IReadOnlyList<MatchSide> CooperativeSides(UiMatchLogDto match)
    {
        var heroes = match.Fighters.Select(ToParticipant).ToList();
        var sides = new List<MatchSide>
            {
                new(null, heroes, heroes.Any(h => h.IsWinner), "Heroes")
            };

        if (match.Villain is not null)
        {
            sides.Add(new MatchSide(Versus, [ToParticipant(match.Villain)], match.Villain.IsWinner, "Villains"));
        }

        return sides;
    }

    private static MatchParticipant ToParticipant(UiFighterDto fighter)
        => new(
            ParticipantKind.Hero,
            fighter.HeroId,
            fighter.Hero?.Name ?? "Unknown",
            fighter.HeroImageUrl,
            fighter.IsWinner,
            fighter.Player?.Name,
            fighter.PlayerImageUrl,
            fighter.Placement,
            fighter.HpLeft,
            fighter.CardsLeft,
            fighter.SidekickName,
            fighter.SidekickHpLeft,
            fighter.MatchPoints);

    private static MatchParticipant ToParticipant(UiMatchVillainDto villain)
        => new(
            ParticipantKind.Villain,
            villain.VillainId,
            villain.Name ?? villain.Villain?.Name ?? "Villain",
            villain.Villain?.ImageUrl ?? "/Unknown.png",
            villain.IsWinner,
            HpLeft: villain.HpLeft,
            CardsLeft: villain.CardsLeft);

    private static MatchParticipant ToParticipant(UiMatchMinionDto minion)
        => new(
            ParticipantKind.Minion,
            minion.MinionId,
            minion.Name ?? minion.Minion?.Name ?? "Minion",
            minion.Minion?.ImageUrl ?? "/Unknown.png",
            minion.IsWinner,
            HpLeft: minion.HpLeft);
}
