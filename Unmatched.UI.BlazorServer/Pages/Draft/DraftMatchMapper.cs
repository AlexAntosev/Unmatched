namespace Unmatched.UI.BlazorServer.Pages.Draft;

using Unmatched.Dtos;
using Unmatched.Enums;

/// <summary>
/// Pure DraftState → UiMatchDto mapping for the draft session's step 5 (design/IMPLEMENTATION-
/// PROMPT.md §5A.5). No DI, no randomness - everything random already happened in earlier steps
/// and is just data on <see cref="DraftState"/> by this point, which makes this straightforward
/// to unit test.
/// </summary>
public static class DraftMatchMapper
{
    public static UiMatchDto ToMatch(DraftState state)
    {
        var turnByPlayer = state.TurnOrder.ToDictionary(t => t.Player.Id, t => t);

        var fighters = state.Players
            .Select(player =>
            {
                var hero = state.PickedHeroesByPlayer.TryGetValue(player.Id, out var picks)
                    ? picks.FirstOrDefault(h => h.Id == state.FinalHeroByPlayer.GetValueOrDefault(player.Id))
                    : null;

                var fighter = new UiFighterDto
                {
                    Player = player,
                    PlayerId = player.Id,
                    Hero = hero,
                    HeroId = hero?.Id ?? Guid.Empty
                };

                if (turnByPlayer.TryGetValue(player.Id, out var turn))
                {
                    fighter.Turn = turn.Turn;
                    fighter.Team = turn.Team;
                }

                fighter.SetDefaultData();
                return fighter;
            })
            .OrderBy(f => f.Turn ?? int.MaxValue)
            .ToList();

        var match = new UiMatchDto
        {
            GameMode = state.Mode,
            Fighters = fighters,
            Map = state.ChosenMap,
            IsRanked = state.Mode != GameMode.Cooperative,
            Date = DateTime.Now,
            Comment = string.Empty
        };

        if (state.IsCoop && state.Villain is not null)
        {
            var villain = new UiMatchVillainDto { Villain = state.Villain, VillainId = state.Villain.Id };
            villain.SetDefaultData(fighters.Count);
            villain.Minions = state.Minions.Select(minion =>
            {
                var matchMinion = new UiMatchMinionDto { Minion = minion, MinionId = minion.Id };
                matchMinion.SetDefaultData();
                return matchMinion;
            }).ToList();

            match.Villain = villain;
        }

        return match;
    }
}
