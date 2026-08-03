namespace Unmatched.MatchService.Tests;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Tournaments;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

/// <summary>Runs BountyChallengeResolver against a real EF Core context (in-memory provider) - same
/// reasoning as TournamentTitleAwarderTests: HeroTitles persistence looks fine against mocks and
/// silently breaks against a real change tracker.</summary>
public class BountyChallengeResolverTests : IDisposable
{
    private readonly UnmatchedDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly BountyChallengeResolver _resolver;

    public BountyChallengeResolverTests()
    {
        var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new UnmatchedDbContext(options);
        _unitOfWork = new UnitOfWork(_dbContext);
        _resolver = new BountyChallengeResolver(_unitOfWork);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task ResolveAsync_NonBountyTournament_DoesNothing()
    {
        var tournamentId = await SeedTournamentAsync(TournamentFormat.SingleElimination);
        var match = CreateMatch(tournamentId, winnerId: Guid.NewGuid(), loserId: Guid.NewGuid());

        await _resolver.ResolveAsync(match);

        Assert.Empty(await _dbContext.TournamentAwards.ToListAsync());
    }

    [Fact]
    public async Task ResolveAsync_UnrankedMatch_AwardsTheChallengeWinner()
    {
        var tournamentId = await SeedTournamentAsync(TournamentFormat.Bounty);
        var winnerId = Guid.NewGuid();
        var match = CreateMatch(tournamentId, winnerId: winnerId, loserId: Guid.NewGuid());

        await _resolver.ResolveAsync(match);

        var award = Assert.Single(await _dbContext.TournamentAwards.ToListAsync());
        Assert.Equal(winnerId, award.HeroId);
        Assert.Equal(TournamentAwardKind.BountyChallengeWin, award.AwardKind);
        Assert.Equal((int)Math.Round(RatingConstants.BountyChallengeWinAwardMultiplier * RatingConstants.KFactor), award.Points);

        var rating = await _dbContext.Ratings.SingleAsync(r => r.HeroId == winnerId);
        Assert.Equal(RatingConstants.InitialRating + award.Points, rating.Points);
    }

    [Fact]
    public async Task ResolveAsync_FirstWin_CreatesTheDynamicHolderTitle()
    {
        var tournamentId = await SeedTournamentAsync(TournamentFormat.Bounty, name: "Iron Cup");
        var winnerId = Guid.NewGuid();
        var match = CreateMatch(tournamentId, winnerId: winnerId, loserId: Guid.NewGuid());

        await _resolver.ResolveAsync(match);

        var title = Assert.Single(await _dbContext.Titles.Include(t => t.HeroTitles).Where(t => t.TournamentId == tournamentId).ToListAsync());
        Assert.Equal("Bounty Iron Cup holder", title.Name);
        Assert.Equal(new[] { winnerId }, title.HeroTitles.Select(h => h.HeroesId));
    }

    [Fact]
    public async Task ResolveAsync_ChampionDefendsAgain_HolderStaysTheSameHero()
    {
        var tournamentId = await SeedTournamentAsync(TournamentFormat.Bounty);
        var championId = Guid.NewGuid();
        await _resolver.ResolveAsync(CreateMatch(tournamentId, winnerId: championId, loserId: Guid.NewGuid(), date: new DateTime(2026, 1, 1)));

        await _resolver.ResolveAsync(CreateMatch(tournamentId, winnerId: championId, loserId: Guid.NewGuid(), date: new DateTime(2026, 1, 8)));

        var title = await _dbContext.Titles.Include(t => t.HeroTitles).FirstAsync(t => t.TournamentId == tournamentId);
        Assert.Equal(new[] { championId }, title.HeroTitles.Select(h => h.HeroesId));

        var awards = await _dbContext.TournamentAwards.Where(a => a.HeroId == championId).ToListAsync();
        Assert.Equal(2, awards.Count);
    }

    [Fact]
    public async Task ResolveAsync_ChallengerWins_HolderTransfers()
    {
        var tournamentId = await SeedTournamentAsync(TournamentFormat.Bounty);
        var championId = Guid.NewGuid();
        var challengerId = Guid.NewGuid();
        await _resolver.ResolveAsync(CreateMatch(tournamentId, winnerId: championId, loserId: Guid.NewGuid(), date: new DateTime(2026, 1, 1)));

        await _resolver.ResolveAsync(CreateMatch(tournamentId, winnerId: challengerId, loserId: championId, date: new DateTime(2026, 1, 8)));

        var title = await _dbContext.Titles.Include(t => t.HeroTitles).FirstAsync(t => t.TournamentId == tournamentId);
        Assert.Equal(new[] { challengerId }, title.HeroTitles.Select(h => h.HeroesId));
    }

    [Fact]
    public async Task ReplayAsync_DoesNotInsertAnotherAwardRow_ButReappliesPointsAndHolder()
    {
        var tournamentId = await SeedTournamentAsync(TournamentFormat.Bounty);
        var winnerId = Guid.NewGuid();
        await _resolver.ResolveAsync(CreateMatch(tournamentId, winnerId: winnerId, loserId: Guid.NewGuid()));
        var award = await _dbContext.TournamentAwards.SingleAsync();

        // simulate RecalculateAsync wiping ratings/titles before replaying the persisted history.
        _dbContext.Ratings.RemoveRange(_dbContext.Ratings);
        _dbContext.HeroTitles.RemoveRange(_dbContext.HeroTitles);
        await _dbContext.SaveChangesAsync();

        await _resolver.ReplayAsync(award);

        Assert.Equal(1, await _dbContext.TournamentAwards.CountAsync());
        var rating = await _dbContext.Ratings.SingleAsync(r => r.HeroId == winnerId);
        Assert.Equal(RatingConstants.InitialRating + award.Points, rating.Points);
        var title = await _dbContext.Titles.Include(t => t.HeroTitles).FirstAsync(t => t.TournamentId == tournamentId);
        Assert.Equal(new[] { winnerId }, title.HeroTitles.Select(h => h.HeroesId));
    }

    private async Task<Guid> SeedTournamentAsync(TournamentFormat format, string name = "Test Bounty")
    {
        var tournament = new TournamentEntity { Id = Guid.NewGuid(), Name = name, Format = format };
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
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerId, IsWinner = true },
                new() { HeroId = loserId, IsWinner = false },
            },
        };
}
