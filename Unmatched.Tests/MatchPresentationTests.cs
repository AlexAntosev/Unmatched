namespace Unmatched.Tests;

using Unmatched.Dtos;
using Unmatched.Enums;
using Unmatched.Services.Statistics;

using static Unmatched.Tests.MatchLogBuilder;

public class MatchPresentationTests
{
    private static UiMatchLogDto CoopMatch(bool villainWon, params string[] minionNames)
    {
        var geralt = Hero("Geralt of Rivia");
        var robin = Hero("Robin Hood");
        var match = Match(
            new DateTime(2026, 7, 5),
            [Fighter(geralt, !villainWon, Player("Vados")), Fighter(robin, !villainWon, Player("Ksuha"))],
            GameMode.Cooperative);

        match.Villain = new UiMatchVillainDto
            {
                VillainId = Guid.NewGuid(),
                Name = "Mothman",
                IsWinner = villainWon,
                HpLeft = 2,
                CardsLeft = 7,
                Minions = minionNames
                    .Select(n => new UiMatchMinionDto { MinionId = Guid.NewGuid(), Name = n, IsWinner = villainWon, HpLeft = 3 })
                    .ToList()
            };

        return match;
    }

    [Fact]
    public void Sides_Duel_ProducesTwoSidesJoinedByVs()
    {
        var match = Duel(new DateTime(2026, 7, 12), Hero("Medusa"), Hero("Alice"), heroWon: true);

        var sides = MatchPresentation.Sides(match);

        Assert.Equal(2, sides.Count);
        Assert.Null(sides[0].Separator);
        Assert.Equal("VS", sides[1].Separator);
        Assert.All(sides, s => Assert.Single(s.Participants));
    }

    [Fact]
    public void Sides_Duel_HasNoSideLabels()
    {
        var match = Duel(new DateTime(2026, 7, 12), Hero("Medusa"), Hero("Alice"), heroWon: true);

        Assert.All(MatchPresentation.Sides(match), s => Assert.Null(s.Label));
    }

    [Fact]
    public void Sides_Team_GroupsByTeamAndLabelsThem()
    {
        var match = Match(
            new DateTime(2026, 7, 11),
            [
                Fighter(Hero("Medusa"), true, team: 1), Fighter(Hero("Dracula"), true, team: 1),
                Fighter(Hero("Alice"), false, team: 2), Fighter(Hero("Bigfoot"), false, team: 2)
            ],
            GameMode.TeamVsTeam);

        var sides = MatchPresentation.Sides(match);

        Assert.Equal(2, sides.Count);
        Assert.Equal("Team 1", sides[0].Label);
        Assert.Equal("Team 2", sides[1].Label);
        Assert.True(sides[0].IsWinner);
        Assert.False(sides[1].IsWinner);
    }

    [Fact]
    public void Sides_FreeForAll_OrdersByPlacementAndUsesChevronSeparator()
    {
        var match = Match(
            new DateTime(2026, 7, 9),
            [
                Fighter(Hero("Third"), false, placement: 3),
                Fighter(Hero("First"), true, placement: 1),
                Fighter(Hero("Second"), false, placement: 2)
            ],
            GameMode.FreeForAll);

        var sides = MatchPresentation.Sides(match);

        Assert.Equal(["First", "Second", "Third"], sides.Select(s => s.Participants.Single().Name));
        Assert.Null(sides[0].Separator);
        Assert.Equal("›", sides[1].Separator);
    }

    [Fact]
    public void Sides_Cooperative_PutsEveryHeroOnOneSideAndTheVillainOnTheOther()
    {
        var sides = MatchPresentation.Sides(CoopMatch(villainWon: true, "Tarantula"));

        Assert.Equal(2, sides.Count);
        Assert.Equal(2, sides[0].Participants.Count);
        Assert.Equal("Heroes", sides[0].Label);
        Assert.Equal("Villains", sides[1].Label);
        Assert.Equal(ParticipantKind.Villain, sides[1].Participants.Single().Kind);
    }

    [Fact]
    public void Sides_Cooperative_MinionsStayOutOfTheCollapsedRow()
    {
        var sides = MatchPresentation.Sides(CoopMatch(villainWon: true, "Tarantula", "Tarantula"));

        Assert.DoesNotContain(sides.SelectMany(s => s.Participants), p => p.Kind == ParticipantKind.Minion);
    }

    [Fact]
    public void Scoreboard_Cooperative_IndentsMinionsUnderTheirVillain()
    {
        var entries = MatchPresentation.Scoreboard(CoopMatch(villainWon: true, "Tarantula", "Tarantula"));

        Assert.Equal(
            ["Geralt of Rivia", "Robin Hood", "Mothman", "Tarantula", "Tarantula"],
            entries.Select(e => e.Participant.Name));
        Assert.False(entries[2].IsIndented);
        Assert.True(entries[3].IsIndented);
        Assert.True(entries[4].IsIndented);
    }

    [Fact]
    public void Scoreboard_Cooperative_LabelsHeroAndVillainSides()
    {
        var entries = MatchPresentation.Scoreboard(CoopMatch(villainWon: false, "Tarantula"));

        Assert.Equal("Heroes", entries[0].SideLabel);
        Assert.Equal("Villains", entries[2].SideLabel);
        Assert.Equal("Villains", entries[3].SideLabel);
    }

    [Fact]
    public void Scoreboard_Duel_HasNoSideLabels()
    {
        var match = Duel(new DateTime(2026, 7, 12), Hero("Medusa"), Hero("Alice"), heroWon: true);

        Assert.All(MatchPresentation.Scoreboard(match), e => Assert.Null(e.SideLabel));
    }

    [Fact]
    public void Scoreboard_FreeForAll_HasNoSideLabelsButKeepsPlaces()
    {
        var match = Match(
            new DateTime(2026, 7, 9),
            [Fighter(Hero("First"), true, placement: 1), Fighter(Hero("Second"), false, placement: 2)],
            GameMode.FreeForAll);

        var entries = MatchPresentation.Scoreboard(match);

        Assert.All(entries, e => Assert.Null(e.SideLabel));
        Assert.Equal([1, 2], entries.Select(e => e.Participant.Place));
    }

    [Fact]
    public void Participants_VillainsAndMinionsCarryNoPlayer()
    {
        var entries = MatchPresentation.Scoreboard(CoopMatch(villainWon: true, "Tarantula"));

        var villainAndMinion = entries.Where(e => e.Participant.Kind != ParticipantKind.Hero).ToList();

        Assert.Equal(2, villainAndMinion.Count);
        Assert.All(villainAndMinion, e => Assert.Null(e.Participant.PlayerName));
        Assert.All(villainAndMinion, e => Assert.Null(e.Participant.PlayerImageUrl));
    }

    [Fact]
    public void Participants_HeroesCarryTheirPlayer()
    {
        var entries = MatchPresentation.Scoreboard(CoopMatch(villainWon: false));

        Assert.Equal("Vados", entries[0].Participant.PlayerName);
        Assert.Equal("Ksuha", entries[1].Participant.PlayerName);
    }

    [Fact]
    public void Participants_HeroesCarryTheirRatingDelta()
    {
        var match = Match(
            new DateTime(2026, 7, 12),
            [Fighter(Hero("Medusa"), true, matchPoints: 18), Fighter(Hero("Alice"), false, matchPoints: -18)],
            GameMode.OneVsOne);

        var entries = MatchPresentation.Scoreboard(match);

        Assert.Equal(18, entries[0].Participant.RatingDelta);
        Assert.Equal(-18, entries[1].Participant.RatingDelta);
    }

    [Fact]
    public void Participants_VillainsAndMinionsCarryNoRatingDelta()
    {
        var entries = MatchPresentation.Scoreboard(CoopMatch(villainWon: true, "Tarantula"));

        var villainAndMinion = entries.Where(e => e.Participant.Kind != ParticipantKind.Hero).ToList();

        Assert.Equal(2, villainAndMinion.Count);
        Assert.All(villainAndMinion, e => Assert.Null(e.Participant.RatingDelta));
    }
}
