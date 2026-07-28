namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.Validation;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

/// <summary>
/// RatingServiceTests covers the replay against mocked repositories; these run it against a real
/// DbContext, because the bug that lost the whole match history only showed up in persistence:
/// recalculation deleted Matches/Fighters and committed that, then re-inserting them failed (the
/// replayed matches kept their original ids, so the write became an UPDATE of rows that no longer
/// existed) and the deletion was never undone.
/// </summary>
public class RatingRecalculationPersistenceTests
{
    private static readonly Guid HeroAId = Guid.NewGuid();
    private static readonly Guid HeroBId = Guid.NewGuid();

    private const int UnrankedWin = 250;
    private const int UnrankedLoss = -150;

    [Fact]
    public async Task RecalculateAsync_KeepsTheWholeMatchHistory()
    {
        var databaseName = await SeedAsync();

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        Assert.Equal(3, await assertContext.Matches.CountAsync());
        Assert.Equal(6, await assertContext.Fighters.CountAsync());
    }

    [Fact]
    public async Task RecalculateAsync_RebuildsRatingsFromScratch_DiscardingWhateverWasThereBefore()
    {
        var databaseName = await SeedAsync();

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        var ratings = await assertContext.Ratings.ToListAsync();

        // one win and one loss each across the two finished matches - the seeded junk rating is gone
        Assert.Equal(2, ratings.Count);
        Assert.Equal(UnrankedWin + UnrankedLoss, ratings.Single(r => r.HeroId == HeroAId).Points);
        Assert.Equal(UnrankedWin + UnrankedLoss, ratings.Single(r => r.HeroId == HeroBId).Points);
    }

    [Fact]
    public async Task RecalculateAsync_LeavesPlannedMatchesUnplayed()
    {
        var databaseName = await SeedAsync();

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        var plannedMatch = await assertContext.Matches.Include(m => m.Fighters).SingleAsync(m => m.IsPlanned);

        Assert.All(plannedMatch.Fighters, fighter => Assert.Null(fighter.MatchPoints));
    }

    [Fact]
    public async Task RecalculateAsync_RewritesMatchPointsOfFinishedMatches()
    {
        var databaseName = await SeedAsync();

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        var fighters = await assertContext.Fighters.Include(f => f.Match).Where(f => !f.Match.IsPlanned).ToListAsync();

        Assert.All(fighters, fighter => Assert.Equal(fighter.IsWinner ? UnrankedWin : UnrankedLoss, fighter.MatchPoints));
    }

    [Fact]
    public async Task RecalculateAsync_ClearsTheRecalculationRequiredFlag()
    {
        var databaseName = await SeedAsync();

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        Assert.False(await new RatingRecalculationStateRepository(assertContext).IsRecalculationRequiredAsync());
    }

    [Fact]
    public async Task RecalculateAsync_MatchesSharingATournament_ReplaysThemAll()
    {
        // every replayed match is written back through the change tracker, so loading each one with its
        // own copy of the tournament it belongs to used to break the second match of any tournament
        var databaseName = Guid.NewGuid().ToString();
        var tournament = new TournamentEntity { Id = Guid.NewGuid(), Name = "Golden Halat League" };

        await using (var seedContext = CreateDbContext(databaseName))
        {
            seedContext.Tournaments.Add(tournament);
            seedContext.Matches.AddRange(
                CreateMatch(new DateTime(2026, 1, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId, tournamentId: tournament.Id),
                CreateMatch(new DateTime(2026, 2, 1), winnerHeroId: HeroBId, looserHeroId: HeroAId, tournamentId: tournament.Id));
            await seedContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        Assert.Equal(2, await assertContext.Matches.CountAsync());
        Assert.Equal(1, await assertContext.Tournaments.CountAsync());
        Assert.Equal(2, await assertContext.Ratings.CountAsync());
    }

    /// <summary>
    /// Two finished unranked 1v1s (each hero wins one) plus a planned one, and a stale rating that the
    /// replay is expected to throw away. Seeds through its own context so the recalculation runs against
    /// a cold change tracker, the way it does behind a request.
    /// </summary>
    private static async Task<string> SeedAsync()
    {
        var databaseName = Guid.NewGuid().ToString();

        await using var dbContext = CreateDbContext(databaseName);
        dbContext.Matches.AddRange(
            CreateMatch(new DateTime(2026, 1, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId),
            CreateMatch(new DateTime(2026, 2, 1), winnerHeroId: HeroBId, looserHeroId: HeroAId),
            CreateMatch(new DateTime(2026, 3, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId, isPlanned: true));
        dbContext.Ratings.Add(new RatingEntity { Id = Guid.NewGuid(), HeroId = HeroAId, Points = 9999 });
        await dbContext.SaveChangesAsync();

        return databaseName;
    }

    private static MatchEntity CreateMatch(DateTime date, Guid winnerHeroId, Guid looserHeroId, bool isPlanned = false, Guid? tournamentId = null)
        => new()
            {
                Id = Guid.NewGuid(),
                Date = date,
                IsPlanned = isPlanned,
                TournamentId = tournamentId,
                Fighters = new List<FighterEntity>
                    {
                        new() { Id = Guid.NewGuid(), HeroId = winnerHeroId, PlayerId = Guid.NewGuid(), IsWinner = true, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 },
                        new() { Id = Guid.NewGuid(), HeroId = looserHeroId, PlayerId = Guid.NewGuid(), IsWinner = false, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 },
                    },
            };

    private static UnmatchedDbContext CreateDbContext(string databaseName)
        => new(new DbContextOptionsBuilder<UnmatchedDbContext>().UseInMemoryDatabase(databaseName).Options);

    private static RatingService CreateRatingService(UnmatchedDbContext dbContext)
    {
        var unitOfWork = new UnitOfWork(dbContext);

        // only the tournament matches score through RatingCalculator and need reference hero data
        var catalogHeroCache = new Mock<ICatalogHeroCache>();
        catalogHeroCache.Setup(c => c.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid heroId) => new CatalogHeroDto { Id = heroId, Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() });

        var matchHandlerFactory = new MatchHandlerFactory(
            unitOfWork,
            new GameModeValidatorFactory(),
            new RatingCalculator(unitOfWork, catalogHeroCache.Object),
            new FirstTournamentRatingCalculator(catalogHeroCache.Object),
            new UnrankedRatingCalculator(),
            new TeamVsTeamRatingCalculator(),
            new FreeForAllRatingCalculator(),
            new CooperativeRatingCalculator());

        return new RatingService(matchHandlerFactory, unitOfWork, new Mock<IMapper>().Object);
    }
}
