namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Tournaments;

public class GroupStageGeneratorTests
{
    private readonly GroupStageGenerator _generator = new();

    [Fact]
    public void GenerateNext_NoGroupMatchesYet_GeneratesTheFullRoundRobinForOneGroupOfFour()
    {
        var participants = CreateParticipants(4);

        var (pairings, stage, round) = _generator.GenerateNext(new TournamentEntity(), participants, []);

        // a round-robin of 4 is every unique pair: 4*3/2 = 6
        Assert.Equal(6, pairings.Count);
        Assert.Equal(Stage.Group, stage);
        Assert.Null(round);
    }

    [Fact]
    public void GenerateNext_EightParticipants_SplitsIntoTwoGroupsOfFour()
    {
        var participants = CreateParticipants(8);

        var (pairings, _, _) = _generator.GenerateNext(new TournamentEntity(), participants, []);

        // two independent groups of 4, each a full round-robin: 6 + 6 = 12
        Assert.Equal(12, pairings.Count);
        // nobody plays a member of the other group
        var pairedHeroIds = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).ToList();
        Assert.Equal(8, pairedHeroIds.Distinct().Count());
    }

    [Fact]
    public void CanGenerateNext_BeforeGroupsExist_ReturnsTrue()
        => Assert.True(_generator.CanGenerateNext(new TournamentEntity(), []));

    [Fact]
    public void GenerateNext_EightParticipants_GroupsFinished_AdvancesTopTwoFromEachActualGroupPlayed()
    {
        // two groups, deliberately interleaved in the participants list - group membership must be
        // reconstructed from the matches actually played (a clique in the "played each other" graph),
        // not re-shuffled from the participants list. A prior bug re-shuffled here, which could split up
        // a group that had already played as a clique and score the wrong heroes against each other.
        var participants = CreateParticipants(8);
        var groupA = new List<TournamentParticipantEntity> { participants[0], participants[2], participants[4], participants[6] };
        var groupB = new List<TournamentParticipantEntity> { participants[1], participants[3], participants[5], participants[7] };
        var groupMatches = RoundRobinResults(groupA).Concat(RoundRobinResults(groupB)).ToList();

        var (pairings, stage, _) = _generator.GenerateNext(new TournamentEntity(), participants, groupMatches);

        Assert.Equal(Stage.SemiFinals, stage);
        var advancing = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).ToList();
        Assert.Equal(4, advancing.Distinct().Count());
        Assert.Contains(groupA[0].HeroId, advancing); // beat everyone in their actual group
        Assert.Contains(groupB[0].HeroId, advancing);
    }

    [Fact]
    public void DeriveGroups_ReconstructsGroupsFromThePairingsPlayed_NotFromParticipantOrder()
    {
        var groupA = CreateParticipants(4);
        var groupB = CreateParticipants(4);
        var matches = RoundRobinResults(groupA).Concat(RoundRobinResults(groupB)).ToList();

        var groups = GroupStageGenerator.DeriveGroups(matches);

        Assert.Equal(2, groups.Count);
        var groupAIds = groupA.Select(p => p.HeroId).OrderBy(id => id).ToList();
        var groupBIds = groupB.Select(p => p.HeroId).OrderBy(id => id).ToList();
        Assert.Contains(groups, g => g.OrderBy(id => id).SequenceEqual(groupAIds));
        Assert.Contains(groups, g => g.OrderBy(id => id).SequenceEqual(groupBIds));
    }

    [Fact]
    public void GenerateNext_GroupsFinished_SeedsAPlayoffFromTheTopTwoOfEachGroup()
    {
        var participants = CreateParticipants(4);
        var groupMatches = RoundRobinResults(participants);
        var tournament = new TournamentEntity { FinalFormat = TournamentFinalFormat.Bo3 };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, participants, groupMatches);

        // top 2 of a single group of 4 advance to a 2-player bracket, which single-elimination seeds
        // directly as the grand final (and, per Bo3, as two identical pairings up front).
        Assert.Equal(Stage.GrandFinals, stage);
        Assert.Equal(2, pairings.Count);
        var winner = participants[0].HeroId; // the participant that beat everyone in RoundRobinResults
        var advancing = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).ToList();
        Assert.Contains(winner, advancing);
    }

    [Fact]
    public void CanGenerateNext_GroupsFinished_ButNoPlayoffYet_ReturnsTrue()
    {
        var participants = CreateParticipants(4);
        var groupMatches = RoundRobinResults(participants);

        Assert.True(_generator.CanGenerateNext(new TournamentEntity(), groupMatches));
    }

    [Fact]
    public void CanGenerateNext_PlayoffGrandFinalsAlreadyGenerated_ReturnsFalse()
    {
        var participants = CreateParticipants(4);
        var matches = RoundRobinResults(participants);
        matches.Add(CreateMatch(Stage.GrandFinals, participants[0].HeroId, participants[1].HeroId));

        Assert.False(_generator.CanGenerateNext(new TournamentEntity(), matches));
    }

    private static List<TournamentParticipantEntity> CreateParticipants(int count)
        => Enumerable.Range(0, count).Select(_ => new TournamentParticipantEntity { HeroId = Guid.NewGuid() }).ToList();

    /// <summary>Every pair in the group has played, with participants[0] beating everybody - a clean
    /// winner for the "top N advance" assertion.</summary>
    private static List<MatchEntity> RoundRobinResults(List<TournamentParticipantEntity> group)
    {
        var matches = new List<MatchEntity>();
        for (var i = 0; i < group.Count; i++)
        {
            for (var j = i + 1; j < group.Count; j++)
            {
                matches.Add(CreateMatch(Stage.Group, group[i].HeroId, group[j].HeroId, winnerIsFirst: i == 0));
            }
        }

        return matches;
    }

    private static MatchEntity CreateMatch(Stage stage, Guid heroA, Guid heroB, bool winnerIsFirst = true)
        => new()
        {
            Stage = stage,
            IsPlanned = false,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = heroA, IsWinner = winnerIsFirst },
                new() { HeroId = heroB, IsWinner = !winnerIsFirst },
            },
        };
}
