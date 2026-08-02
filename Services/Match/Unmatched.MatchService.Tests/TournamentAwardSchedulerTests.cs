namespace Unmatched.MatchService.Tests;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Tournaments;

/// <summary>Reproduces the plan's worked self-funded-pool examples exactly (8-player playoff pool=320,
/// entry=40, Winner nets +88, etc.) so the constants in RatingConstants and this scheduler stay in sync
/// with the documented design.</summary>
public class TournamentAwardSchedulerTests
{
    private static readonly DateTime AwardedAt = new(2026, 1, 1);

    private readonly TournamentAwardScheduler _scheduler = new();

    [Fact]
    public void Schedule_SingleEliminationFourPlayers_MatchesTheWorkedExample()
    {
        var h = CreateHeroes(4);
        var matches = new List<MatchEntity>
        {
            CreateMatch(Stage.SemiFinals, h[0], h[2]),
            CreateMatch(Stage.SemiFinals, h[1], h[3]),
            CreateMatch(Stage.GrandFinals, h[0], h[1]),
            CreateMatch(Stage.GrandFinals, h[0], h[1]),
        };
        var tournament = CreateTournament(TournamentFormat.SingleElimination, h);

        var schedule = _scheduler.Schedule(tournament, matches, AwardedAt);

        Assert.Equal(32, PointsFor(schedule, h[0]));
        Assert.Equal(0, PointsFor(schedule, h[1]));
        Assert.Equal(-16, PointsFor(schedule, h[2]));
        Assert.Equal(-16, PointsFor(schedule, h[3]));
        Assert.Equal(0, schedule.Awards.Sum(a => a.Points));
    }

    [Fact]
    public void Schedule_SingleEliminationEightPlayers_MatchesTheWorkedExample()
    {
        var h = CreateHeroes(8);
        var matches = new List<MatchEntity>
        {
            CreateMatch(Stage.QuarterFinals, h[0], h[4]),
            CreateMatch(Stage.QuarterFinals, h[1], h[5]),
            CreateMatch(Stage.QuarterFinals, h[2], h[6]),
            CreateMatch(Stage.QuarterFinals, h[3], h[7]),
            CreateMatch(Stage.SemiFinals, h[0], h[2]),
            CreateMatch(Stage.SemiFinals, h[1], h[3]),
            CreateMatch(Stage.ThirdPlaceDecider, h[2], h[3]),
            CreateMatch(Stage.GrandFinals, h[0], h[1]),
            CreateMatch(Stage.GrandFinals, h[0], h[1]),
        };
        var tournament = CreateTournament(TournamentFormat.SingleElimination, h);

        var schedule = _scheduler.Schedule(tournament, matches, AwardedAt);

        Assert.Equal(88, PointsFor(schedule, h[0]));
        Assert.Equal(24, PointsFor(schedule, h[1]));
        Assert.Equal(-8, PointsFor(schedule, h[2]));
        Assert.Equal(-8, PointsFor(schedule, h[3]));
        Assert.Equal(-24, PointsFor(schedule, h[4]));
        Assert.Equal(-24, PointsFor(schedule, h[5]));
        Assert.Equal(-24, PointsFor(schedule, h[6]));
        Assert.Equal(-24, PointsFor(schedule, h[7]));
        Assert.Equal(0, schedule.Awards.Sum(a => a.Points));

        Assert.Equal(1, schedule.FinalPlacements[h[0]]);
        Assert.Equal(2, schedule.FinalPlacements[h[1]]);
        Assert.Equal(3, schedule.FinalPlacements[h[2]]);
        Assert.Equal(4, schedule.FinalPlacements[h[3]]);
    }

    [Fact]
    public void Schedule_SingleEliminationSixteenPlayers_MatchesTheWorkedExample()
    {
        var h = CreateHeroes(16);
        var matches = new List<MatchEntity>();
        for (var i = 0; i < 8; i++)
        {
            matches.Add(CreateMatch(Stage.EighthFinals, h[i], h[i + 8]));
        }

        matches.Add(CreateMatch(Stage.QuarterFinals, h[0], h[2]));
        matches.Add(CreateMatch(Stage.QuarterFinals, h[1], h[3]));
        matches.Add(CreateMatch(Stage.QuarterFinals, h[4], h[6]));
        matches.Add(CreateMatch(Stage.QuarterFinals, h[5], h[7]));
        matches.Add(CreateMatch(Stage.SemiFinals, h[0], h[1]));
        matches.Add(CreateMatch(Stage.SemiFinals, h[4], h[5]));
        matches.Add(CreateMatch(Stage.ThirdPlaceDecider, h[1], h[4]));
        matches.Add(CreateMatch(Stage.GrandFinals, h[0], h[4]));
        matches.Add(CreateMatch(Stage.GrandFinals, h[0], h[4]));

        var tournament = CreateTournament(TournamentFormat.SingleElimination, h);

        var schedule = _scheduler.Schedule(tournament, matches, AwardedAt);

        Assert.Equal(156, PointsFor(schedule, h[0]));
        Assert.Equal(60, PointsFor(schedule, h[4]));
        Assert.Equal(12, PointsFor(schedule, h[1]));
        Assert.Equal(12, PointsFor(schedule, h[5]));
        foreach (var quarterfinalLoser in new[] { h[2], h[3], h[6], h[7] })
        {
            Assert.Equal(-12, PointsFor(schedule, quarterfinalLoser));
        }

        for (var i = 8; i < 16; i++)
        {
            Assert.Equal(-24, PointsFor(schedule, h[i]));
        }

        Assert.Equal(0, schedule.Awards.Sum(a => a.Points));
    }

    [Fact]
    public void Schedule_GroupStage_GroupWinnerBonusStacksWithThePlayoffTier()
    {
        var h = CreateHeroes(4);
        var groupMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.Group, h[0], h[1]),
            CreateMatch(Stage.Group, h[0], h[2]),
            CreateMatch(Stage.Group, h[0], h[3]),
            CreateMatch(Stage.Group, h[1], h[2]),
            CreateMatch(Stage.Group, h[1], h[3]),
            CreateMatch(Stage.Group, h[2], h[3]),
        };
        var playoffMatches = new List<MatchEntity>
        {
            CreateMatch(Stage.GrandFinals, h[0], h[1]),
            CreateMatch(Stage.GrandFinals, h[0], h[1]),
        };
        var tournament = CreateTournament(TournamentFormat.GroupStage, h);

        var schedule = _scheduler.Schedule(tournament, groupMatches.Concat(playoffMatches).ToList(), AwardedAt);

        // hero 0 topped the group (3-0) AND won the tournament - two separate award rows, one hero.
        var heroZeroAwards = schedule.Awards.Where(a => a.HeroId == h[0]).ToList();
        Assert.Equal(2, heroZeroAwards.Count);
        Assert.Contains(heroZeroAwards, a => a.AwardKind == TournamentAwardKind.Winner);
        Assert.Contains(heroZeroAwards, a => a.AwardKind == TournamentAwardKind.GroupWinner);
        Assert.Equal(52, heroZeroAwards.Sum(a => a.Points));

        Assert.Equal(4, PointsFor(schedule, h[1]));
        Assert.Equal(-28, PointsFor(schedule, h[2]));
        Assert.Equal(-28, PointsFor(schedule, h[3]));
        Assert.Equal(0, schedule.Awards.Sum(a => a.Points));
    }

    [Fact]
    public void Schedule_League_TopThreeGetTheSeasonSchedule_AndEverySumIsZero()
    {
        var h = CreateHeroes(5);
        var matches = new List<MatchEntity>();
        AddWins(matches, h[0], 4);
        AddWins(matches, h[1], 3);
        AddLosses(matches, h[1], 1);
        AddWins(matches, h[2], 2);
        AddLosses(matches, h[2], 2);
        AddWins(matches, h[3], 1);
        AddLosses(matches, h[3], 3);
        AddLosses(matches, h[4], 4);

        var tournament = CreateTournament(TournamentFormat.League, h);

        var schedule = _scheduler.Schedule(tournament, matches, AwardedAt);

        var kindByHero = schedule.Awards.ToDictionary(a => a.HeroId, a => a.AwardKind);
        Assert.Equal(TournamentAwardKind.SeasonFirst, kindByHero[h[0]]);
        Assert.Equal(TournamentAwardKind.SeasonSecond, kindByHero[h[1]]);
        Assert.Equal(TournamentAwardKind.SeasonThird, kindByHero[h[2]]);
        Assert.Equal(TournamentAwardKind.Eliminated, kindByHero[h[3]]);
        Assert.Equal(TournamentAwardKind.Eliminated, kindByHero[h[4]]);
        Assert.Equal(1, schedule.FinalPlacements[h[0]]);
        Assert.Equal(5, schedule.FinalPlacements[h[4]]);
        Assert.Equal(0, schedule.Awards.Sum(a => a.Points));
    }

    [Fact]
    public void Schedule_Swiss_ReusesTheBracketTierNames_AndEverySumIsZero()
    {
        var h = CreateHeroes(8);
        var matches = new List<MatchEntity>();
        AddWins(matches, h[0], 4);
        AddWins(matches, h[1], 3);
        AddWins(matches, h[2], 2);
        AddWins(matches, h[3], 1);
        // heroes 4-7 stay winless - only ranks 0-3 get a tier, so their tie doesn't affect this assertion

        var tournament = CreateTournament(TournamentFormat.Swiss, h);

        var schedule = _scheduler.Schedule(tournament, matches, AwardedAt);

        var kindByHero = schedule.Awards.ToDictionary(a => a.HeroId, a => a.AwardKind);
        Assert.Equal(TournamentAwardKind.Winner, kindByHero[h[0]]);
        Assert.Equal(TournamentAwardKind.Finalist, kindByHero[h[1]]);
        Assert.Equal(TournamentAwardKind.Semifinalist, kindByHero[h[2]]);
        // top half beyond the top three still earns the Quarterfinalist ("top half") tier
        Assert.Equal(TournamentAwardKind.Quarterfinalist, kindByHero[h[3]]);
        Assert.Equal(0, schedule.Awards.Sum(a => a.Points));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(8)]
    [InlineData(11)]
    [InlineData(16)]
    public void Schedule_League_AnyParticipantCount_AwardsSumToExactlyZero(int participantCount)
    {
        var h = CreateHeroes(participantCount);
        var matches = new List<MatchEntity>();
        for (var i = 0; i < h.Count; i++)
        {
            AddWins(matches, h[i], h.Count - i);
            AddLosses(matches, h[i], i);
        }

        var tournament = CreateTournament(TournamentFormat.League, h);

        var schedule = _scheduler.Schedule(tournament, matches, AwardedAt);

        Assert.Equal(0, schedule.Awards.Sum(a => a.Points));
    }

    [Fact]
    public void Schedule_BountyFormat_Throws()
    {
        var h = CreateHeroes(2);
        var tournament = CreateTournament(TournamentFormat.Bounty, h);

        Assert.Throws<InvalidOperationException>(() => _scheduler.Schedule(tournament, [], AwardedAt));
    }

    [Fact]
    public void Schedule_SingleEliminationWithNoGrandFinalPlayed_Throws()
    {
        var h = CreateHeroes(4);
        var matches = new List<MatchEntity> { CreateMatch(Stage.SemiFinals, h[0], h[2]), CreateMatch(Stage.SemiFinals, h[1], h[3]) };
        var tournament = CreateTournament(TournamentFormat.SingleElimination, h);

        Assert.Throws<InvalidOperationException>(() => _scheduler.Schedule(tournament, matches, AwardedAt));
    }

    [Fact]
    public void Schedule_LeagueWithNoFinishedMatches_Throws()
    {
        var h = CreateHeroes(3);
        var tournament = CreateTournament(TournamentFormat.League, h);

        Assert.Throws<InvalidOperationException>(() => _scheduler.Schedule(tournament, [], AwardedAt));
    }

    private static int PointsFor(TournamentCompletionSchedule schedule, Guid heroId)
        => schedule.Awards.Where(a => a.HeroId == heroId).Sum(a => a.Points);

    private static List<Guid> CreateHeroes(int count)
        => Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();

    private static TournamentEntity CreateTournament(TournamentFormat format, IEnumerable<Guid> heroIds)
        => new()
        {
            Id = Guid.NewGuid(),
            Format = format,
            Participants = heroIds.Select(id => new TournamentParticipantEntity { HeroId = id }).ToList()
        };

    /// <summary>A win/loss against a throwaway opponent id, not one of the tracked participants - League
    /// and Swiss standings only tally the participants they're given, so this is enough to hand a hero an
    /// exact win/loss count without needing a realistic round robin.</summary>
    private static void AddWins(List<MatchEntity> matches, Guid heroId, int count)
    {
        for (var i = 0; i < count; i++)
        {
            matches.Add(CreateMatch(null, heroId, Guid.NewGuid()));
        }
    }

    private static void AddLosses(List<MatchEntity> matches, Guid heroId, int count)
    {
        for (var i = 0; i < count; i++)
        {
            matches.Add(CreateMatch(null, Guid.NewGuid(), heroId));
        }
    }

    private static MatchEntity CreateMatch(Stage? stage, Guid winnerId, Guid loserId)
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
