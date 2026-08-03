namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Titles.Rules;
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

    private static readonly CatalogHeroDto ReferenceHero = new() { Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() };

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

        var (_, expectedAPoints, expectedBPoints) = ExpectedRatingsAfterJanThenFeb();

        await using var assertContext = CreateDbContext(databaseName);
        var ratings = await assertContext.Ratings.ToListAsync();

        // one win and one loss each across the two finished (Ranked) matches - the seeded junk rating is gone
        Assert.Equal(2, ratings.Count);
        Assert.Equal(expectedAPoints, ratings.Single(r => r.HeroId == HeroAId).Points);
        Assert.Equal(expectedBPoints, ratings.Single(r => r.HeroId == HeroBId).Points);
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

        var (janDelta, _, _) = ExpectedRatingsAfterJanThenFeb();

        await using var assertContext = CreateDbContext(databaseName);
        var janMatch = await assertContext.Matches.Include(m => m.Fighters).SingleAsync(m => m.Date == new DateTime(2026, 1, 1));

        Assert.All(janMatch.Fighters, fighter => Assert.Equal(fighter.IsWinner ? janDelta : -janDelta, fighter.MatchPoints));
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

    [Fact]
    public async Task RecalculateAsync_UnrankedMatch_ContributesNoPointsAndCreatesNoRatingRow()
    {
        var databaseName = Guid.NewGuid().ToString();

        await using (var seedContext = CreateDbContext(databaseName))
        {
            seedContext.Matches.Add(CreateMatch(new DateTime(2026, 1, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId, isRanked: false));
            await seedContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        Assert.Empty(await assertContext.Ratings.ToListAsync());
        var fighters = await assertContext.Fighters.ToListAsync();
        Assert.All(fighters, fighter => Assert.Equal(0, fighter.MatchPoints));
    }

    /// <summary>
    /// Ranked-data completeness (non-null HP/cards/sidekick HP) is enforced on the save path in
    /// <c>MatchService.AddOrUpdateAsync</c>, not inside <see cref="IMatchHandler"/> - deliberately, so
    /// that a recalculation replaying matches saved before that rule existed never trips over them.
    /// This seeds exactly that: a Ranked match with no stats recorded at all.
    /// </summary>
    [Fact]
    public async Task RecalculateAsync_RankedMatchWithMissingStats_DoesNotThrow()
    {
        var databaseName = Guid.NewGuid().ToString();

        await using (var seedContext = CreateDbContext(databaseName))
        {
            seedContext.Matches.Add(CreateMatch(new DateTime(2026, 1, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId, statsRecorded: false));
            await seedContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        Assert.Equal(2, await assertContext.Ratings.CountAsync());
    }

    [Fact]
    public async Task RecalculateAsync_TournamentAwardsSurviveARecalculation()
    {
        var databaseName = Guid.NewGuid().ToString();
        var tournamentId = Guid.NewGuid();

        await using (var seedContext = CreateDbContext(databaseName))
        {
            seedContext.Tournaments.Add(new TournamentEntity { Id = tournamentId, Name = "Test Tournament" });
            seedContext.Matches.Add(CreateMatch(new DateTime(2026, 1, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId));
            seedContext.TournamentAwards.Add(CreateAward(tournamentId, HeroAId, points: 50, awardedAt: new DateTime(2026, 2, 1)));
            await seedContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        var matchDelta = EloRating.Delta(RatingConstants.InitialRating, RatingConstants.InitialRating, ReferencePerformance());

        await using var assertContext = CreateDbContext(databaseName);
        var heroARating = await assertContext.Ratings.SingleAsync(r => r.HeroId == HeroAId);
        Assert.Equal(RatingConstants.InitialRating + matchDelta + 50, heroARating.Points);
    }

    /// <summary>
    /// The core reason matches and awards must be replayed through one merged timeline rather than two
    /// separate passes: Elo's expected score depends on the rating *at that instant*, so an award dated
    /// between two matches changes the second match's delta.
    /// </summary>
    [Fact]
    public async Task RecalculateAsync_AwardDatedMidHistory_ChangesTheLaterMatchsDelta()
    {
        var databaseName = Guid.NewGuid().ToString();
        var tournamentId = Guid.NewGuid();
        const int awardPoints = 60;

        await using (var seedContext = CreateDbContext(databaseName))
        {
            seedContext.Tournaments.Add(new TournamentEntity { Id = tournamentId, Name = "Test Tournament" });
            seedContext.Matches.AddRange(
                CreateMatch(new DateTime(2026, 1, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId),
                CreateMatch(new DateTime(2026, 3, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId));
            seedContext.TournamentAwards.Add(CreateAward(tournamentId, HeroAId, points: awardPoints, awardedAt: new DateTime(2026, 2, 1)));
            await seedContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateDbContext(databaseName))
        {
            await CreateRatingService(dbContext).RecalculateAsync();
        }

        var performance = ReferencePerformance();
        var janDelta = EloRating.Delta(RatingConstants.InitialRating, RatingConstants.InitialRating, performance);
        var aAfterJan = RatingConstants.InitialRating + janDelta;
        var bAfterJan = RatingConstants.InitialRating - janDelta;
        var aAfterAward = aAfterJan + awardPoints;

        var marchDeltaWithAward = EloRating.Delta(aAfterAward, bAfterJan, performance);
        var marchDeltaIgnoringAward = EloRating.Delta(aAfterJan, bAfterJan, performance);
        Assert.NotEqual(marchDeltaIgnoringAward, marchDeltaWithAward);

        await using var assertContext = CreateDbContext(databaseName);
        var marchMatch = await assertContext.Matches.Include(m => m.Fighters).SingleAsync(m => m.Date == new DateTime(2026, 3, 1));
        Assert.Equal(marchDeltaWithAward, marchMatch.Fighters.Single(f => f.HeroId == HeroAId).MatchPoints);

        var heroARating = await assertContext.Ratings.SingleAsync(r => r.HeroId == HeroAId);
        Assert.Equal(aAfterAward + marchDeltaWithAward, heroARating.Points);
    }

    /// <summary>
    /// GrandChampion reads the live Ratings table ("whoever holds #1 right now"), the same class of
    /// dependency Elo itself has on replay order - so a recalculation must recompute it in step with the
    /// ratings, not leave whatever stale holder was there before. This seeds Hero A as the (now wrong)
    /// incumbent, an award that puts Hero B in the lead, and a later match to re-trigger the rule - title
    /// rules only re-run on match events (mirroring <c>MatchService.AddOrUpdateAsync</c>, which is the
    /// only place they run in production too), so the award alone doesn't flip the holder until the next
    /// match sees the post-award ratings.
    /// </summary>
    [Fact]
    public async Task RecalculateAsync_RecomputesRatingDependentTitles_DiscardingStaleHolders()
    {
        var databaseName = Guid.NewGuid().ToString();
        var titleId = Guid.NewGuid();
        var tournamentId = Guid.NewGuid();

        await using (var seedContext = CreateDbContext(databaseName))
        {
            seedContext.Titles.Add(new TitleEntity
            {
                Id = titleId,
                Name = "Grand Champion",
                Comment = string.Empty,
                Exclusivity = TitleExclusivity.Unique,
                RuleKey = Titles.GrandChampion,
                HeroTitles = new List<HeroTitleEntity>
                {
                    new() { HeroesId = HeroAId, TitlesId = titleId, EarnedAt = new DateTime(2026, 1, 1) }
                }
            });
            seedContext.Tournaments.Add(new TournamentEntity { Id = tournamentId, Name = "Test Tournament" });
            seedContext.Matches.AddRange(
                CreateMatch(new DateTime(2026, 1, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId),
                CreateMatch(new DateTime(2026, 3, 1), winnerHeroId: HeroAId, looserHeroId: HeroBId));
            seedContext.TournamentAwards.Add(CreateAward(tournamentId, HeroBId, points: 500, awardedAt: new DateTime(2026, 2, 1)));
            await seedContext.SaveChangesAsync();
        }

        await using (var dbContext = CreateDbContext(databaseName))
        {
            var unitOfWork = new UnitOfWork(dbContext);
            var catalogHeroCache = new Mock<ICatalogHeroCache>();
            catalogHeroCache.Setup(c => c.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid heroId) => new CatalogHeroDto { Id = heroId, Hp = ReferenceHero.Hp, DeckSize = ReferenceHero.DeckSize, Sidekicks = ReferenceHero.Sidekicks });

            var matchHandler = new MatchHandler(unitOfWork, new GameModeValidatorFactory(), new RatingCalculatorFactory(unitOfWork, catalogHeroCache.Object));
            var titleEvaluator = new TitleEvaluator(unitOfWork, new Mock<IMapper>().Object, new ITitleRule[] { new GrandChampionTitleRule(unitOfWork) });
            var titleAwarder = new TournamentTitleAwarder(unitOfWork, catalogHeroCache.Object);
            var bountyChallengeResolver = new Domain.Tournaments.BountyChallengeResolver(unitOfWork);
            var ratingService = new RatingService(matchHandler, unitOfWork, new Mock<IMapper>().Object, new RatingTimeline(unitOfWork), titleEvaluator, titleAwarder, bountyChallengeResolver);

            await ratingService.RecalculateAsync();
        }

        await using var assertContext = CreateDbContext(databaseName);
        var holders = await assertContext.HeroTitles.Where(ht => ht.TitlesId == titleId).Select(ht => ht.HeroesId).ToListAsync();
        Assert.Equal(new[] { HeroBId }, holders);
    }

    private static TournamentAwardEntity CreateAward(Guid tournamentId, Guid heroId, int points, DateTime awardedAt)
        => new()
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            HeroId = heroId,
            AwardKind = TournamentAwardKind.Winner,
            Points = points,
            AwardedAt = awardedAt
        };

    /// <summary>Replays the same Jan-then-Feb 1v1 sequence <see cref="SeedAsync"/> creates through the
    /// production Elo primitives directly, so the persistence assertions above stay correct even if
    /// the tuning constants in <see cref="RatingConstants"/> change.</summary>
    private static (int JanDelta, int HeroAFinalPoints, int HeroBFinalPoints) ExpectedRatingsAfterJanThenFeb()
    {
        var performance = ReferencePerformance();
        var janDelta = EloRating.Delta(RatingConstants.InitialRating, RatingConstants.InitialRating, performance);
        var aAfterJan = RatingConstants.InitialRating + janDelta;
        var bAfterJan = RatingConstants.InitialRating - janDelta;

        // February: Hero B beats Hero A
        var febDelta = EloRating.Delta(bAfterJan, aAfterJan, performance);
        var aFinal = aAfterJan - febDelta;
        var bFinal = bAfterJan + febDelta;

        return (janDelta, aFinal, bFinal);
    }

    private static double ReferencePerformance()
    {
        var fullyResolvedFighter = new FighterEntity { HpLeft = 0, CardsLeft = 0, SidekickHpLeft = 0 };
        return PerformanceModifier.Calculate(
            winningSide: [PerformanceModifier.From(fullyResolvedFighter, ReferenceHero)],
            losingSide: [PerformanceModifier.From(fullyResolvedFighter, ReferenceHero)]);
    }

    /// <summary>
    /// Two finished Ranked 1v1s (each hero wins one) plus a planned one, and a stale rating that the
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

    private static MatchEntity CreateMatch(
        DateTime date,
        Guid winnerHeroId,
        Guid looserHeroId,
        bool isPlanned = false,
        bool isRanked = true,
        bool statsRecorded = true,
        Guid? tournamentId = null)
        => new()
            {
                Id = Guid.NewGuid(),
                Date = date,
                GameMode = GameMode.OneVsOne,
                IsPlanned = isPlanned,
                IsRanked = isRanked,
                TournamentId = tournamentId,
                Fighters = new List<FighterEntity>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(), HeroId = winnerHeroId, PlayerId = Guid.NewGuid(), IsWinner = true,
                            HpLeft = statsRecorded ? 0 : null, SidekickHpLeft = statsRecorded ? 0 : null, CardsLeft = statsRecorded ? 0 : null
                        },
                        new()
                        {
                            Id = Guid.NewGuid(), HeroId = looserHeroId, PlayerId = Guid.NewGuid(), IsWinner = false,
                            HpLeft = statsRecorded ? 0 : null, SidekickHpLeft = statsRecorded ? 0 : null, CardsLeft = statsRecorded ? 0 : null
                        },
                    },
            };

    private static UnmatchedDbContext CreateDbContext(string databaseName)
        => new(new DbContextOptionsBuilder<UnmatchedDbContext>().UseInMemoryDatabase(databaseName).Options);

    private static RatingService CreateRatingService(UnmatchedDbContext dbContext)
    {
        var unitOfWork = new UnitOfWork(dbContext);

        var catalogHeroCache = new Mock<ICatalogHeroCache>();
        catalogHeroCache.Setup(c => c.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid heroId) => new CatalogHeroDto { Id = heroId, Hp = ReferenceHero.Hp, DeckSize = ReferenceHero.DeckSize, Sidekicks = ReferenceHero.Sidekicks });

        var matchHandler = new MatchHandler(
            unitOfWork,
            new GameModeValidatorFactory(),
            new RatingCalculatorFactory(unitOfWork, catalogHeroCache.Object));

        var titleEvaluator = new TitleEvaluator(unitOfWork, new Mock<IMapper>().Object, []);
        var titleAwarder = new TournamentTitleAwarder(unitOfWork, catalogHeroCache.Object);

        var bountyChallengeResolver = new Domain.Tournaments.BountyChallengeResolver(unitOfWork);
        return new RatingService(matchHandler, unitOfWork, new Mock<IMapper>().Object, new RatingTimeline(unitOfWork), titleEvaluator, titleAwarder, bountyChallengeResolver);
    }
}
