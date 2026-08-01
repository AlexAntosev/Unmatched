namespace Unmatched.Tests;

using Unmatched.Dtos;
using Unmatched.Enums;
using Unmatched.UI.BlazorServer.Pages.Draft;

using static Unmatched.Tests.MatchLogBuilder;

/// <summary>
/// DraftState -&gt; UiMatchDto is pure and non-random by the time step 5 runs (every random pool
/// already resolved into plain data on the state) - these fix the state by hand and assert on the
/// resulting match, one mode at a time.
/// </summary>
public class DraftMatchMapperTests
{
    private static readonly UiPlayerDto Andrii = Player("Andrii");
    private static readonly UiPlayerDto Oleksandr = Player("Oleksandr");
    private static readonly UiHeroDto Geralt = Hero("Geralt of Rivia");
    private static readonly UiHeroDto Medusa = Hero("Medusa");
    private static readonly MapDto Castle = new() { Id = Guid.NewGuid(), Name = "Castle" };

    private static DraftState BuildOneVsOneState()
    {
        var state = new DraftState { Mode = GameMode.OneVsOne, ChosenMap = Castle };
        state.Players.Add(Andrii);
        state.Players.Add(Oleksandr);
        state.PickedHeroesByPlayer[Andrii.Id] = [Geralt];
        state.PickedHeroesByPlayer[Oleksandr.Id] = [Medusa];
        state.FinalHeroByPlayer[Andrii.Id] = Geralt.Id;
        state.FinalHeroByPlayer[Oleksandr.Id] = Medusa.Id;
        state.TurnOrder = [(Oleksandr, 1, null), (Andrii, 2, null)];
        return state;
    }

    [Fact]
    public void ToMatch_OneVsOne_MapsGameModeAndMap()
    {
        var match = DraftMatchMapper.ToMatch(BuildOneVsOneState());

        Assert.Equal(GameMode.OneVsOne, match.GameMode);
        Assert.Same(Castle, match.Map);
        Assert.True(match.IsRanked);
        Assert.Null(match.Villain);
    }

    [Fact]
    public void ToMatch_OneVsOne_OrdersFightersByTurnAndAssignsFinalHero()
    {
        var match = DraftMatchMapper.ToMatch(BuildOneVsOneState());

        var fighters = match.Fighters.ToList();
        Assert.Equal(2, fighters.Count);
        Assert.Equal(Oleksandr.Id, fighters[0].PlayerId);
        Assert.Equal(Medusa.Id, fighters[0].HeroId);
        Assert.Equal(1, fighters[0].Turn);
        Assert.Equal(Andrii.Id, fighters[1].PlayerId);
        Assert.Equal(Geralt.Id, fighters[1].HeroId);
        Assert.Equal(2, fighters[1].Turn);
    }

    [Fact]
    public void ToMatch_OneVsOne_FillsStatsFromTheFinalHero()
    {
        // SetDefaultData() reads HP/deck size/sidekicks off Hero - a non-zero deck size proves the
        // mapper actually ran it, not just copied the hero reference across.
        Geralt.DeckSize = 30;
        Geralt.Hp = 16;

        var match = DraftMatchMapper.ToMatch(BuildOneVsOneState());

        var geraltFighter = match.Fighters.Single(f => f.HeroId == Geralt.Id);
        Assert.Equal(30, geraltFighter.CardsLeft);
        Assert.Equal(16, geraltFighter.HpLeft);
    }

    [Fact]
    public void ToMatch_TeamVsTeam_CarriesTeamFromTurnOrderOntoEachFighter()
    {
        var vados = Player("Vados");
        var ksuha = Player("Ksuha");
        var state = new DraftState { Mode = GameMode.TeamVsTeam, ChosenMap = Castle };
        state.Players.AddRange([Andrii, Oleksandr, vados, ksuha]);
        foreach (var (player, hero) in new[] { (Andrii, Geralt), (Oleksandr, Medusa), (vados, Geralt), (ksuha, Medusa) })
        {
            state.PickedHeroesByPlayer[player.Id] = [hero];
            state.FinalHeroByPlayer[player.Id] = hero.Id;
        }

        // Team-alternating numbers (design/IMPLEMENTATION-PROMPT.md §5.4): team 1 -> 1,3, team 2 -> 2,4.
        state.TurnOrder =
        [
            (Andrii, 1, 1), (vados, 3, 1),
            (Oleksandr, 2, 2), (ksuha, 4, 2)
        ];

        var match = DraftMatchMapper.ToMatch(state);

        var byPlayer = match.Fighters.ToDictionary(f => f.PlayerId);
        Assert.Equal(1, byPlayer[Andrii.Id].Team);
        Assert.Equal(1, byPlayer[vados.Id].Team);
        Assert.Equal(2, byPlayer[Oleksandr.Id].Team);
        Assert.Equal(2, byPlayer[ksuha.Id].Team);
    }

    [Fact]
    public void ToMatch_Cooperative_IsUnrankedAndHasNoTurnOrder()
    {
        var state = new DraftState { Mode = GameMode.Cooperative, ChosenMap = Castle };
        state.Players.Add(Andrii);
        state.Players.Add(Oleksandr);
        state.PickedHeroesByPlayer[Andrii.Id] = [Geralt];
        state.PickedHeroesByPlayer[Oleksandr.Id] = [Medusa];
        state.FinalHeroByPlayer[Andrii.Id] = Geralt.Id;
        state.FinalHeroByPlayer[Oleksandr.Id] = Medusa.Id;
        // Co-op never rolls a turn order (design/IMPLEMENTATION-PROMPT.md §5.4) - state.TurnOrder
        // stays empty, matching what VillainStep leaves it at.

        var match = DraftMatchMapper.ToMatch(state);

        Assert.False(match.IsRanked);
        Assert.All(match.Fighters, f => Assert.Null(f.Turn));
    }

    [Fact]
    public void ToMatch_Cooperative_MapsVillainAndMinions()
    {
        var villain = new VillainDto { Id = Guid.NewGuid(), Name = "Bloody Mary", BaseHp = 10, HpPerExtraPlayer = 5 };
        var ant = new MinionDto { Id = Guid.NewGuid(), Name = "Ant Queen", Hp = 10 };
        var state = new DraftState { Mode = GameMode.Cooperative, ChosenMap = Castle, Villain = villain };
        state.Players.Add(Andrii);
        state.Players.Add(Oleksandr);
        state.PickedHeroesByPlayer[Andrii.Id] = [Geralt];
        state.PickedHeroesByPlayer[Oleksandr.Id] = [Medusa];
        state.FinalHeroByPlayer[Andrii.Id] = Geralt.Id;
        state.FinalHeroByPlayer[Oleksandr.Id] = Medusa.Id;
        state.Minions.Add(ant);

        var match = DraftMatchMapper.ToMatch(state);

        Assert.NotNull(match.Villain);
        Assert.Equal(villain.Id, match.Villain!.VillainId);
        Assert.Equal(15, match.Villain.HpLeft); // EffectiveHp(2): BaseHp 10 + HpPerExtraPlayer 5 * (2-1)
        var minion = Assert.Single(match.Villain.Minions);
        Assert.Equal(ant.Id, minion.MinionId);
        Assert.Equal(ant.Hp, minion.HpLeft);
    }
}
