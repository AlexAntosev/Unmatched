namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Tournaments;

public class SwissGeneratorTests
{
    private readonly SwissGenerator _generator = new();

    [Fact]
    public void GenerateNext_FirstRound_PairsUpEveryParticipant_AsRoundOne()
    {
        var participants = CreateParticipants(4);

        var (pairings, stage, round) = _generator.GenerateNext(new TournamentEntity(), participants, []);

        Assert.Equal(2, pairings.Count);
        Assert.Null(stage);
        Assert.Equal(1, round);
    }

    [Fact]
    public void GenerateNext_OddFieldSize_LeavesExactlyOneHeroWithoutAPairing()
    {
        var participants = CreateParticipants(5);

        var (pairings, _, _) = _generator.GenerateNext(new TournamentEntity(), participants, []);

        // 5 participants -> 2 pairings (4 heroes) + 1 bye, not 2.5 pairings
        Assert.Equal(2, pairings.Count);
    }

    [Fact]
    public void GenerateNext_RoundNumber_IsOneMoreThanTheHighestRoundAlreadyPlayed()
    {
        var participants = CreateParticipants(4);
        var existingMatches = participants
            .Chunk(2)
            .Select(pair => CreateMatch(pair[0].HeroId, pair[1].HeroId, round: 3))
            .ToList();

        var (_, _, round) = _generator.GenerateNext(new TournamentEntity(), participants, existingMatches);

        Assert.Equal(4, round);
    }

    [Fact]
    public void GenerateNext_AvoidsRematches_WhenAnAlternativePairingExists()
    {
        var a = new TournamentParticipantEntity { HeroId = Guid.NewGuid() };
        var b = new TournamentParticipantEntity { HeroId = Guid.NewGuid() };
        var c = new TournamentParticipantEntity { HeroId = Guid.NewGuid() };
        var d = new TournamentParticipantEntity { HeroId = Guid.NewGuid() };
        var participants = new List<TournamentParticipantEntity> { a, b, c, d };

        // round 1: A beat B, C beat D - so A/B and C/D have already played each other
        var existingMatches = new List<MatchEntity>
        {
            CreateMatch(a.HeroId, b.HeroId, round: 1, winnerIsFirst: true),
            CreateMatch(c.HeroId, d.HeroId, round: 1, winnerIsFirst: true),
        };

        var (pairings, _, _) = _generator.GenerateNext(new TournamentEntity(), participants, existingMatches);

        Assert.DoesNotContain(pairings, p => IsPair(p, a.HeroId, b.HeroId));
        Assert.DoesNotContain(pairings, p => IsPair(p, c.HeroId, d.HeroId));
    }

    [Fact]
    public void CanGenerateNext_AlwaysReturnsTrue_TheOrganiserDecidesWhenToStop()
        => Assert.True(_generator.CanGenerateNext(new TournamentEntity(), []));

    private static bool IsPair(TournamentPairing pairing, Guid a, Guid b)
        => (pairing.HeroId == a && pairing.OpponentHeroId == b) || (pairing.HeroId == b && pairing.OpponentHeroId == a);

    private static List<TournamentParticipantEntity> CreateParticipants(int count)
        => Enumerable.Range(0, count).Select(_ => new TournamentParticipantEntity { HeroId = Guid.NewGuid() }).ToList();

    private static MatchEntity CreateMatch(Guid heroA, Guid heroB, int round, bool winnerIsFirst = true)
        => new()
        {
            Round = round,
            IsPlanned = false,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = heroA, IsWinner = winnerIsFirst },
                new() { HeroId = heroB, IsWinner = !winnerIsFirst },
            },
        };
}
