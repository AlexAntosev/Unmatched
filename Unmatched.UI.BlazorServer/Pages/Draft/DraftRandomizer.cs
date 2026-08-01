namespace Unmatched.UI.BlazorServer.Pages.Draft;

using Unmatched.Dtos;
using Unmatched.Enums;
using Unmatched.Extensions;
using Unmatched.UI.BlazorServer.Shared.TurnOrder;

/// <summary>
/// Pure pool/order generation for the draft session (design/IMPLEMENTATION-PROMPT.md §5A). No DI,
/// no component state - callers pass already-loaded, owned-set-filtered catalogs in and get back
/// plain lists, so this is straightforward to unit test structurally without mocking the RNG.
/// </summary>
public static class DraftRandomizer
{
    /// <summary>N+1 random maps for N players - one map survives every player's single ban.</summary>
    public static List<MapDto> BuildMapPool(IReadOnlyList<MapDto> ownedMaps, int playerCount)
        => ownedMaps.ToList().Shuffle().Take(playerCount + 1).ToList();

    /// <summary>N×3 random heroes for N players - one ban each, then a snake pick leaves exactly
    /// two heroes per player.</summary>
    public static List<UiHeroDto> BuildHeroPool(IReadOnlyList<UiHeroDto> ownedHeroes, int playerCount)
        => ownedHeroes.ToList().Shuffle().Take(playerCount * 3).ToList();

    /// <summary>Random order in which players act - used for both the map-ban and hero-ban phases,
    /// rolled independently for each.</summary>
    public static List<UiPlayerDto> RollBanOrder(IReadOnlyList<UiPlayerDto> players)
        => players.ToList().Shuffle();

    /// <summary>Snake pick: forward through the ban order, then back, so every player picks
    /// exactly twice regardless of how many players are drafting.</summary>
    public static List<UiPlayerDto> SnakePickOrder(IReadOnlyList<UiPlayerDto> banOrder)
        => banOrder.Concat(banOrder.Reverse()).ToList();

    /// <summary>Splits players into two 2-a-side teams, keeping any hand-picked T1/T2 badge from
    /// the Players step and randomly assigning whoever hasn't been toggled.</summary>
    public static void SplitTeamsRandomly(IReadOnlyList<UiPlayerDto> players, IDictionary<Guid, int> teams)
    {
        teams.Clear();
        var shuffled = players.ToList().Shuffle();
        for (var i = 0; i < shuffled.Count; i++)
        {
            teams[shuffled[i].Id] = i % 2 == 0 ? 1 : 2;
        }
    }

    /// <summary>1v1/FFA: one flat random order. 2v2: teams alternate turns (team 1 → 1,3, team 2 →
    /// 2,4), each team shuffled independently - the same rule MatchSheet's drag-confined-to-team
    /// renumber uses, via the shared <see cref="TeamAlternatingTurns"/> helper.</summary>
    public static List<(UiPlayerDto Player, int Turn, int? Team)> RollTurnOrder(
        GameMode mode, IReadOnlyList<UiPlayerDto> players, IReadOnlyDictionary<Guid, int> teams)
    {
        if (mode == GameMode.TeamVsTeam)
        {
            var team1 = players.Where(p => teams.GetValueOrDefault(p.Id, 1) == 1).ToList().Shuffle();
            var team2 = players.Where(p => teams.GetValueOrDefault(p.Id, 1) == 2).ToList().Shuffle();

            var result = new List<(UiPlayerDto, int, int?)>();
            for (var i = 0; i < team1.Count; i++)
            {
                result.Add((team1[i], TeamAlternatingTurns.TurnFor(1, i), 1));
            }

            for (var i = 0; i < team2.Count; i++)
            {
                result.Add((team2[i], TeamAlternatingTurns.TurnFor(2, i), 2));
            }

            return result;
        }

        var order = players.ToList().Shuffle();
        return order.Select((p, i) => (p, i + 1, (int?)null)).ToList();
    }
}
