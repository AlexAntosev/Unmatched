namespace Unmatched.MatchService.Tests;

using Microsoft.EntityFrameworkCore;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

/// <summary>Runs BountyRatingCalculator against a real EF Core context (in-memory provider) - same
/// reasoning as other Bounty tests this project: HeroTitles/TournamentAwards persistence looks fine
/// against mocks and silently breaks against a real change tracker. Every fighter is built
/// fully-resolved (0 HP/cards/sidekick HP on both sides) against a fixed reference hero, so the Elo
/// delta N for any given pair of ratings is reproducible via <see cref="ReferenceDelta"/>.</summary>
public class BountyRatingCalculatorTests : IDisposable
{
    private static readonly CatalogHeroDto ReferenceHero = new() { Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() };

    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly UnmatchedDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly BountyRatingCalculator _calculator;

    public BountyRatingCalculatorTests()
    {
        _dbContext = CreateDbContext();
        _unitOfWork = new UnitOfWork(_dbContext);

        var catalogHeroCache = new Mock<ICatalogHeroCache>();
        catalogHeroCache.Setup(c => c.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid heroId) => new CatalogHeroDto { Id = heroId, Hp = ReferenceHero.Hp, DeckSize = ReferenceHero.DeckSize, Sidekicks = ReferenceHero.Sidekicks });

        _calculator = new BountyRatingCalculator(_unitOfWork, catalogHeroCache.Object);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task CalculateAsync_FirstChallengeNoStartingChampion_TreatedAsDethroneOfAVacantTitle()
    {
        var tournamentId = await SeedTournamentAsync();
        var winnerId = Guid.NewGuid();
        var loserId = Guid.NewGuid();
        var match = CreateMatch(tournamentId, winnerId, loserId);

        var result = await _calculator.CalculateAsync(match);
        await _unitOfWork.SaveChangesAsync();

        var n = ReferenceDelta(RatingConstants.InitialRating, RatingConstants.InitialRating);
        Assert.Equal(n / 2, result[winnerId]); // dethrone payout is winnerShare + bank, and the bank was 0
        Assert.Equal(-n, result[loserId]);

        var pool = await _dbContext.TournamentAwards.SingleAsync(a => a.AwardKind == TournamentAwardKind.BountyPool);
        Assert.Equal(winnerId, pool.HeroId);
        Assert.Equal(n - n / 2, pool.Points);
    }

    [Fact]
    public async Task CalculateAsync_FirstChallengeWinnerMatchesStartingChampion_IsADefense()
    {
        var championId = Guid.NewGuid();
        var tournamentId = await SeedTournamentAsync(championId);
        var challengerId = Guid.NewGuid();
        var match = CreateMatch(tournamentId, championId, challengerId);

        var result = await _calculator.CalculateAsync(match);
        await _unitOfWork.SaveChangesAsync();

        var n = ReferenceDelta(RatingConstants.InitialRating, RatingConstants.InitialRating);
        Assert.Equal(n / 2, result[championId]);
        Assert.Equal(-n, result[challengerId]);

        var pool = await _dbContext.TournamentAwards.SingleAsync(a => a.AwardKind == TournamentAwardKind.BountyPool);
        Assert.Equal(championId, pool.HeroId);
        Assert.Equal(n - n / 2, pool.Points);
    }

    [Fact]
    public async Task CalculateAsync_FirstChallengeWinnerDiffersFromStartingChampion_IsADethrone()
    {
        var championId = Guid.NewGuid();
        var tournamentId = await SeedTournamentAsync(championId);
        var challengerId = Guid.NewGuid();
        var match = CreateMatch(tournamentId, challengerId, championId);

        var result = await _calculator.CalculateAsync(match);
        await _unitOfWork.SaveChangesAsync();

        var n = ReferenceDelta(RatingConstants.InitialRating, RatingConstants.InitialRating);
        Assert.Equal(n / 2, result[challengerId]); // bank was 0, so dethrone payout N/2 + 0 == a defense's payout here
        Assert.Equal(-n, result[championId]);

        var pool = await _dbContext.TournamentAwards.SingleAsync(a => a.AwardKind == TournamentAwardKind.BountyPool);
        Assert.Equal(challengerId, pool.HeroId);
    }

    [Fact]
    public async Task CalculateAsync_Dethrone_WinnerGetsHalfPlusTheWholeBank_AndBankReseedsUnderTheNewHolder()
    {
        var championId = Guid.NewGuid();
        var tournamentId = await SeedTournamentAsync(championId);

        var firstChallengerId = Guid.NewGuid();
        var firstMatch = CreateMatch(tournamentId, championId, firstChallengerId, date: new DateTime(2026, 1, 1));
        await _calculator.CalculateAsync(firstMatch);
        await _unitOfWork.SaveChangesAsync();
        var firstDelta = ReferenceDelta(RatingConstants.InitialRating, RatingConstants.InitialRating);
        var bankAfterFirstDefense = firstDelta - firstDelta / 2;
        var championRatingAfterFirst = RatingConstants.InitialRating + firstDelta / 2;

        var dethronerId = Guid.NewGuid();
        var dethroneMatch = CreateMatch(tournamentId, dethronerId, championId, date: new DateTime(2026, 1, 8));
        var result = await _calculator.CalculateAsync(dethroneMatch);
        await _unitOfWork.SaveChangesAsync();

        var secondDelta = ReferenceDelta(RatingConstants.InitialRating, championRatingAfterFirst);
        var expectedWinnerShare = secondDelta / 2;

        Assert.Equal(expectedWinnerShare + bankAfterFirstDefense, result[dethronerId]);
        Assert.Equal(-secondDelta, result[championId]);

        var pool = await _dbContext.TournamentAwards.SingleAsync(a => a.AwardKind == TournamentAwardKind.BountyPool);
        Assert.Equal(dethronerId, pool.HeroId);
        Assert.Equal(secondDelta - expectedWinnerShare, pool.Points);
    }

    [Fact]
    public async Task CalculateAsync_SecondCall_ReadsHolderAndBankFromThePersistedRow_NotStartingChampionIdAgain()
    {
        var startingChampionId = Guid.NewGuid();
        var tournamentId = await SeedTournamentAsync(startingChampionId);

        var dethronerId = Guid.NewGuid();
        var dethroneMatch = CreateMatch(tournamentId, dethronerId, startingChampionId, date: new DateTime(2026, 1, 1));
        await _calculator.CalculateAsync(dethroneMatch);
        await _unitOfWork.SaveChangesAsync();

        // The old starting champion challenges again - if the calculator wrongly re-consulted
        // StartingChampionId instead of the pool row, this would be misread as a defense.
        var rematch = CreateMatch(tournamentId, startingChampionId, dethronerId, date: new DateTime(2026, 1, 8));
        var result = await _calculator.CalculateAsync(rematch);

        var pool = await _dbContext.TournamentAwards.SingleAsync(a => a.AwardKind == TournamentAwardKind.BountyPool);
        var bankBeforeRematch = pool.Points;
        var n = ReferenceDelta(RatingConstants.InitialRating, RatingConstants.InitialRating);
        Assert.Equal(n / 2 + bankBeforeRematch, result[startingChampionId]);
    }

    [Fact]
    public async Task CalculateAsync_StagesThePoolUpsert_ButDoesNotCommitUntilAnExplicitSave()
    {
        var tournamentId = await SeedTournamentAsync();
        var match = CreateMatch(tournamentId, Guid.NewGuid(), Guid.NewGuid());

        await _calculator.CalculateAsync(match);

        await using var unsavedReadContext = CreateDbContext();
        Assert.False(await unsavedReadContext.TournamentAwards.AnyAsync(a => a.AwardKind == TournamentAwardKind.BountyPool));

        await _unitOfWork.SaveChangesAsync();

        await using var savedReadContext = CreateDbContext();
        Assert.True(await savedReadContext.TournamentAwards.AnyAsync(a => a.AwardKind == TournamentAwardKind.BountyPool));
    }

    private UnmatchedDbContext CreateDbContext()
        => new(new DbContextOptionsBuilder<UnmatchedDbContext>().UseInMemoryDatabase(_databaseName).Options);

    private async Task<Guid> SeedTournamentAsync(Guid? startingChampionId = null)
    {
        var tournament = new TournamentEntity
        {
            Id = Guid.NewGuid(), Name = "Test Bounty", Format = TournamentFormat.Bounty, StartingChampionId = startingChampionId
        };
        _dbContext.Tournaments.Add(tournament);
        await _dbContext.SaveChangesAsync();
        return tournament.Id;
    }

    private static MatchEntity CreateMatch(Guid tournamentId, Guid winnerId, Guid loserId, DateTime? date = null)
        => new()
        {
            Id = Guid.NewGuid(),
            TournamentId = tournamentId,
            Date = date ?? new DateTime(2026, 1, 1),
            GameMode = GameMode.OneVsOne,
            IsRanked = true,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerId, IsWinner = true, HpLeft = 0, CardsLeft = 0, SidekickHpLeft = 0 },
                new() { HeroId = loserId, IsWinner = false, HpLeft = 0, CardsLeft = 0, SidekickHpLeft = 0 },
            },
        };

    private static double ReferencePerformance()
    {
        var fullyResolvedFighter = new FighterEntity { HpLeft = 0, CardsLeft = 0, SidekickHpLeft = 0 };
        return PerformanceModifier.Calculate(
            winningSide: [PerformanceModifier.From(fullyResolvedFighter, ReferenceHero)],
            losingSide: [PerformanceModifier.From(fullyResolvedFighter, ReferenceHero)]);
    }

    private static int ReferenceDelta(int winnerRating, int loserRating)
        => EloRating.Delta(winnerRating, loserRating, ReferencePerformance());
}
