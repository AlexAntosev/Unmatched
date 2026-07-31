namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Tournaments;

public class SingleEliminationGeneratorTests
{
    private readonly SingleEliminationGenerator _generator = new();

    [Fact]
    public void GenerateNext_FirstRound_PairsUpEveryParticipant_AtTheTournamentsInitialStage()
    {
        var participants = CreateParticipants(4);
        var tournament = new TournamentEntity { CurrentStage = Stage.SemiFinals };

        var (pairings, stage, round) = _generator.GenerateNext(tournament, participants, []);

        Assert.Equal(2, pairings.Count);
        Assert.Equal(Stage.SemiFinals, stage);
        Assert.Null(round);
        var pairedHeroIds = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).ToList();
        Assert.Equal(participants.Select(p => p.HeroId).OrderBy(id => id), pairedHeroIds.OrderBy(id => id));
    }

    [Fact]
    public void GenerateNext_LaterRound_AdvancesWinnersOnly()
    {
        var winnerA = Guid.NewGuid();
        var loserA = Guid.NewGuid();
        var winnerB = Guid.NewGuid();
        var loserB = Guid.NewGuid();

        var tournament = new TournamentEntity { CurrentStage = Stage.QuarterFinals };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.QuarterFinals, winnerA, loserA),
            CreateMatch(Stage.QuarterFinals, winnerB, loserB),
        };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, [], existingMatches);

        Assert.Equal(Stage.SemiFinals, stage);
        var advancing = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).ToList();
        Assert.Contains(winnerA, advancing);
        Assert.Contains(winnerB, advancing);
        Assert.DoesNotContain(loserA, advancing);
        Assert.DoesNotContain(loserB, advancing);
    }

    [Fact]
    public void GenerateNext_AfterSemiFinals_ThirdPlaceDeciderIsBetweenTheLosers()
    {
        var winnerA = Guid.NewGuid();
        var loserA = Guid.NewGuid();
        var winnerB = Guid.NewGuid();
        var loserB = Guid.NewGuid();

        var tournament = new TournamentEntity { CurrentStage = Stage.SemiFinals };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.SemiFinals, winnerA, loserA),
            CreateMatch(Stage.SemiFinals, winnerB, loserB),
        };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, [], existingMatches);

        Assert.Equal(Stage.ThirdPlaceDecider, stage);
        var thirdPlacePlayers = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).ToList();
        Assert.Contains(loserA, thirdPlacePlayers);
        Assert.Contains(loserB, thirdPlacePlayers);
    }

    [Fact]
    public void GenerateNext_AfterThirdPlaceDecider_GrandFinalsIsSeededFromSemiFinalWinners_NotTheThirdPlaceResult()
    {
        var winnerA = Guid.NewGuid();
        var loserA = Guid.NewGuid();
        var winnerB = Guid.NewGuid();
        var loserB = Guid.NewGuid();

        var tournament = new TournamentEntity { CurrentStage = Stage.ThirdPlaceDecider };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.SemiFinals, winnerA, loserA),
            CreateMatch(Stage.SemiFinals, winnerB, loserB),
            // whoever wins the third-place decider must not affect who reaches the grand final
            CreateMatch(Stage.ThirdPlaceDecider, loserB, loserA),
        };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, [], existingMatches);

        Assert.Equal(Stage.GrandFinals, stage);
        var finalists = pairings.SelectMany(p => new[] { p.HeroId, p.OpponentHeroId }).ToList();
        Assert.Contains(winnerA, finalists);
        Assert.Contains(winnerB, finalists);
    }

    [Fact]
    public void GenerateNext_GrandFinals_ProducesThreeIdenticalPairings_TheBestOfThreeStandIn()
    {
        var finalistA = Guid.NewGuid();
        var finalistB = Guid.NewGuid();

        var tournament = new TournamentEntity { CurrentStage = Stage.ThirdPlaceDecider };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.SemiFinals, finalistA, Guid.NewGuid()),
            CreateMatch(Stage.SemiFinals, finalistB, Guid.NewGuid()),
            CreateMatch(Stage.ThirdPlaceDecider, Guid.NewGuid(), Guid.NewGuid()),
        };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, [], existingMatches);

        Assert.Equal(Stage.GrandFinals, stage);
        Assert.Equal(3, pairings.Count);
        Assert.All(pairings, p => Assert.Equal(pairings[0], p));
    }

    [Fact]
    public void CanGenerateNext_NoMatchesYet_ReturnsTrue()
        => Assert.True(_generator.CanGenerateNext(new TournamentEntity(), []));

    [Fact]
    public void CanGenerateNext_GrandFinalsAlreadyGenerated_ReturnsFalse()
    {
        var existingMatches = new List<MatchEntity> { CreateMatch(Stage.GrandFinals, Guid.NewGuid(), Guid.NewGuid()) };

        Assert.False(_generator.CanGenerateNext(new TournamentEntity(), existingMatches));
    }

    private static List<TournamentParticipantEntity> CreateParticipants(int count)
        => Enumerable.Range(0, count).Select(_ => new TournamentParticipantEntity { HeroId = Guid.NewGuid() }).ToList();

    private static MatchEntity CreateMatch(Stage stage, Guid winnerId, Guid loserId)
        => new()
        {
            Stage = stage,
            IsPlanned = false,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerId, IsWinner = true },
                new() { HeroId = loserId, IsWinner = false },
            },
        };
}
