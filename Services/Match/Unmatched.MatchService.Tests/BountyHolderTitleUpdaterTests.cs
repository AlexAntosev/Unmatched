namespace Unmatched.MatchService.Tests;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Tournaments;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

/// <summary>Runs BountyHolderTitleUpdater against a real EF Core context (in-memory provider) -
/// HeroTitles persistence looks fine against mocks and silently breaks against a real change
/// tracker.</summary>
public class BountyHolderTitleUpdaterTests : IDisposable
{
    private readonly UnmatchedDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly BountyHolderTitleUpdater _updater;

    public BountyHolderTitleUpdaterTests()
    {
        var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new UnmatchedDbContext(options);
        _unitOfWork = new UnitOfWork(_dbContext);
        _updater = new BountyHolderTitleUpdater(_unitOfWork);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task UpdateAsync_FirstWin_CreatesTheDynamicHolderTitle()
    {
        var tournament = await SeedTournamentAsync(name: "Iron Cup");
        var winnerId = Guid.NewGuid();
        var match = CreateMatch(tournament.Id, winnerId, loserId: Guid.NewGuid());

        await _updater.UpdateAsync(tournament, match);
        await _dbContext.SaveChangesAsync();

        var title = Assert.Single(await _dbContext.Titles.Include(t => t.HeroTitles).Where(t => t.TournamentId == tournament.Id).ToListAsync());
        Assert.Equal("Bounty Iron Cup holder", title.Name);
        Assert.Equal(new[] { winnerId }, title.HeroTitles.Select(h => h.HeroesId));
    }

    [Fact]
    public async Task UpdateAsync_ChampionDefendsAgain_HolderStaysTheSameHero()
    {
        var tournament = await SeedTournamentAsync();
        var championId = Guid.NewGuid();
        await _updater.UpdateAsync(tournament, CreateMatch(tournament.Id, championId, Guid.NewGuid(), new DateTime(2026, 1, 1)));
        await _dbContext.SaveChangesAsync();

        await _updater.UpdateAsync(tournament, CreateMatch(tournament.Id, championId, Guid.NewGuid(), new DateTime(2026, 1, 8)));
        await _dbContext.SaveChangesAsync();

        var title = await _dbContext.Titles.Include(t => t.HeroTitles).FirstAsync(t => t.TournamentId == tournament.Id);
        Assert.Equal(new[] { championId }, title.HeroTitles.Select(h => h.HeroesId));
    }

    [Fact]
    public async Task UpdateAsync_ChallengerWins_HolderTransfers()
    {
        var tournament = await SeedTournamentAsync();
        var championId = Guid.NewGuid();
        var challengerId = Guid.NewGuid();
        await _updater.UpdateAsync(tournament, CreateMatch(tournament.Id, championId, Guid.NewGuid(), new DateTime(2026, 1, 1)));
        await _dbContext.SaveChangesAsync();

        await _updater.UpdateAsync(tournament, CreateMatch(tournament.Id, challengerId, championId, new DateTime(2026, 1, 8)));
        await _dbContext.SaveChangesAsync();

        var title = await _dbContext.Titles.Include(t => t.HeroTitles).FirstAsync(t => t.TournamentId == tournament.Id);
        Assert.Equal(new[] { challengerId }, title.HeroTitles.Select(h => h.HeroesId));
    }

    private async Task<TournamentEntity> SeedTournamentAsync(string name = "Test Bounty")
    {
        var tournament = new TournamentEntity { Id = Guid.NewGuid(), Name = name, Format = TournamentFormat.Bounty };
        _dbContext.Tournaments.Add(tournament);
        await _dbContext.SaveChangesAsync();
        return tournament;
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
