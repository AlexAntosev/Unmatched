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
    public void GenerateNext_GrandFinals_Bo1_ProducesOneGame()
    {
        var finalistA = Guid.NewGuid();
        var finalistB = Guid.NewGuid();

        var tournament = new TournamentEntity { CurrentStage = Stage.ThirdPlaceDecider, FinalFormat = TournamentFinalFormat.Bo1 };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.SemiFinals, finalistA, Guid.NewGuid()),
            CreateMatch(Stage.SemiFinals, finalistB, Guid.NewGuid()),
            CreateMatch(Stage.ThirdPlaceDecider, Guid.NewGuid(), Guid.NewGuid()),
        };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, [], existingMatches);

        Assert.Equal(Stage.GrandFinals, stage);
        Assert.Single(pairings);
    }

    [Fact]
    public void GenerateNext_GrandFinals_Bo3_ProducesTwoIdenticalGamesUpFront()
    {
        var finalistA = Guid.NewGuid();
        var finalistB = Guid.NewGuid();

        var tournament = new TournamentEntity { CurrentStage = Stage.ThirdPlaceDecider, FinalFormat = TournamentFinalFormat.Bo3 };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.SemiFinals, finalistA, Guid.NewGuid()),
            CreateMatch(Stage.SemiFinals, finalistB, Guid.NewGuid()),
            CreateMatch(Stage.ThirdPlaceDecider, Guid.NewGuid(), Guid.NewGuid()),
        };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, [], existingMatches);

        Assert.Equal(Stage.GrandFinals, stage);
        Assert.Equal(2, pairings.Count);
        Assert.Equal(pairings[0], pairings[1]);
    }

    [Fact]
    public void GenerateNext_Bo3TiedOneGameEach_SchedulesDeciderBetweenTheSameTwoFinalists()
    {
        var finalistA = Guid.NewGuid();
        var finalistB = Guid.NewGuid();

        var tournament = new TournamentEntity { CurrentStage = Stage.GrandFinals, FinalFormat = TournamentFinalFormat.Bo3 };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.GrandFinals, finalistA, finalistB),
            CreateMatch(Stage.GrandFinals, finalistB, finalistA),
        };

        var (pairings, stage, _) = _generator.GenerateNext(tournament, [], existingMatches);

        Assert.Equal(Stage.GrandFinals, stage);
        var decider = Assert.Single(pairings);
        Assert.Equal(new HashSet<Guid> { finalistA, finalistB }, new HashSet<Guid> { decider.HeroId, decider.OpponentHeroId });
    }

    [Fact]
    public void CanGenerateNext_NoMatchesYet_ReturnsTrue()
        => Assert.True(_generator.CanGenerateNext(new TournamentEntity(), []));

    [Fact]
    public void CanGenerateNext_Bo1GrandFinalPlayed_ReturnsFalse()
    {
        var tournament = new TournamentEntity { FinalFormat = TournamentFinalFormat.Bo1 };
        var existingMatches = new List<MatchEntity> { CreateMatch(Stage.GrandFinals, Guid.NewGuid(), Guid.NewGuid()) };

        Assert.False(_generator.CanGenerateNext(tournament, existingMatches));
    }

    [Fact]
    public void CanGenerateNext_Bo3SweptTwoNil_ReturnsFalse()
    {
        var finalistA = Guid.NewGuid();
        var finalistB = Guid.NewGuid();
        var tournament = new TournamentEntity { FinalFormat = TournamentFinalFormat.Bo3 };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.GrandFinals, finalistA, finalistB),
            CreateMatch(Stage.GrandFinals, finalistA, finalistB),
        };

        Assert.False(_generator.CanGenerateNext(tournament, existingMatches));
    }

    [Fact]
    public void CanGenerateNext_Bo3TiedOneGameEach_ReturnsTrue()
    {
        var finalistA = Guid.NewGuid();
        var finalistB = Guid.NewGuid();
        var tournament = new TournamentEntity { FinalFormat = TournamentFinalFormat.Bo3 };
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.GrandFinals, finalistA, finalistB),
            CreateMatch(Stage.GrandFinals, finalistB, finalistA),
        };

        Assert.True(_generator.CanGenerateNext(tournament, existingMatches));
    }

    [Fact]
    public void CanGenerateNext_Bo3TiedButSecondGameStillPlanned_ReturnsFalse()
    {
        var finalistA = Guid.NewGuid();
        var finalistB = Guid.NewGuid();
        var tournament = new TournamentEntity { FinalFormat = TournamentFinalFormat.Bo3 };
        var plannedGame = CreateMatch(Stage.GrandFinals, finalistB, finalistA);
        plannedGame.IsPlanned = true;
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.GrandFinals, finalistA, finalistB),
            plannedGame,
        };

        Assert.False(_generator.CanGenerateNext(tournament, existingMatches));
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
