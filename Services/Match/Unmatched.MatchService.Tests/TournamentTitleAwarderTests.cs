namespace Unmatched.MatchService.Tests;

using Microsoft.EntityFrameworkCore;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

/// <summary>Runs TournamentTitleAwarder against a real EF Core context (in-memory provider) - same
/// reasoning as TitleEvaluatorTests: HeroTitles persistence looks fine against mocks and silently breaks
/// against a real change tracker.</summary>
public class TournamentTitleAwarderTests : IDisposable
{
    private static readonly CatalogHeroDto ReferenceHero = new() { Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() };

    private readonly UnmatchedDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();
    private readonly TournamentTitleAwarder _awarder;

    public TournamentTitleAwarderTests()
    {
        var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new UnmatchedDbContext(options);
        _unitOfWork = new UnitOfWork(_dbContext);
        _catalogHeroCache.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsAsync(ReferenceHero);

        _awarder = new TournamentTitleAwarder(_unitOfWork, _catalogHeroCache.Object);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task AwardAsync_OnlyAwardsTheKindsSelectedAtCreation()
    {
        var championId = Guid.NewGuid();
        var runnerUpId = Guid.NewGuid();
        var tournament = CreateTournament(
            titleKinds: [TournamentTitleKind.Champion],
            participants: [(championId, 1), (runnerUpId, 2)]);

        await _awarder.AwardAsync(tournament, []);
        await _unitOfWork.SaveChangesAsync();

        var titles = await _dbContext.Titles.Where(t => t.TournamentId == tournament.Id).ToListAsync();
        Assert.Single(titles);
        Assert.Contains("Champion", titles[0].Name);
    }

    [Fact]
    public async Task AwardAsync_Champion_GoesToTheFirstPlacedParticipant()
    {
        var championId = Guid.NewGuid();
        var runnerUpId = Guid.NewGuid();
        var tournament = CreateTournament(
            titleKinds: [TournamentTitleKind.Champion, TournamentTitleKind.RunnerUp],
            participants: [(championId, 1), (runnerUpId, 2)]);

        await _awarder.AwardAsync(tournament, []);
        await _unitOfWork.SaveChangesAsync();

        Assert.Equal(new[] { championId }, await HolderIdsAsync(tournament.Id, "Champion"));
        Assert.Equal(new[] { runnerUpId }, await HolderIdsAsync(tournament.Id, "Runner-Up"));
    }

    [Fact]
    public async Task AwardAsync_IronChin_GoesToWhoeverLostTheMostHp()
    {
        var toughHeroId = Guid.NewGuid();
        var frailHeroId = Guid.NewGuid();
        var tournament = CreateTournament(
            titleKinds: [TournamentTitleKind.IronChin],
            participants: [(toughHeroId, 1), (frailHeroId, 2)]);
        var matches = new List<MatchEntity>
        {
            OneVOneMatch(Fighter(toughHeroId, hpLeft: 14), Fighter(frailHeroId, hpLeft: 2)),
        };

        await _awarder.AwardAsync(tournament, matches);
        await _unitOfWork.SaveChangesAsync();

        Assert.Equal(new[] { frailHeroId }, await HolderIdsAsync(tournament.Id, "Iron Chin"));
    }

    [Fact]
    public async Task AwardAsync_IronChin_SkipsFightersWithNoRecordedHp()
    {
        var recordedId = Guid.NewGuid();
        var unrecordedId = Guid.NewGuid();
        var tournament = CreateTournament(
            titleKinds: [TournamentTitleKind.IronChin],
            participants: [(recordedId, 1), (unrecordedId, 2)]);
        var matches = new List<MatchEntity>
        {
            OneVOneMatch(Fighter(recordedId, hpLeft: 10), Fighter(unrecordedId, hpLeft: null)),
        };

        await _awarder.AwardAsync(tournament, matches);
        await _unitOfWork.SaveChangesAsync();

        // only recordedId has any measurable HP lost - the unrecorded fighter can't win a title off no data.
        Assert.Equal(new[] { recordedId }, await HolderIdsAsync(tournament.Id, "Iron Chin"));
    }

    [Fact]
    public async Task AwardAsync_CardShark_GoesToWhoeverUsedTheMostCards()
    {
        var sparingHeroId = Guid.NewGuid();
        var heavyHandedHeroId = Guid.NewGuid();
        var tournament = CreateTournament(
            titleKinds: [TournamentTitleKind.CardShark],
            participants: [(sparingHeroId, 1), (heavyHandedHeroId, 2)]);
        var matches = new List<MatchEntity>
        {
            OneVOneMatch(Fighter(sparingHeroId, cardsLeft: 8), Fighter(heavyHandedHeroId, cardsLeft: 1)),
        };

        await _awarder.AwardAsync(tournament, matches);
        await _unitOfWork.SaveChangesAsync();

        Assert.Equal(new[] { heavyHandedHeroId }, await HolderIdsAsync(tournament.Id, "Card Shark"));
    }

    [Fact]
    public async Task AwardAsync_Executioner_GoesToWhoeverDealtTheMostHp()
    {
        var dealerId = Guid.NewGuid();
        var victimId = Guid.NewGuid();
        var tournament = CreateTournament(
            titleKinds: [TournamentTitleKind.Executioner],
            participants: [(dealerId, 1), (victimId, 2)]);
        var matches = new List<MatchEntity>
        {
            OneVOneMatch(Fighter(dealerId, hpLeft: 16), Fighter(victimId, hpLeft: 0)),
        };

        await _awarder.AwardAsync(tournament, matches);
        await _unitOfWork.SaveChangesAsync();

        Assert.Equal(new[] { dealerId }, await HolderIdsAsync(tournament.Id, "Executioner"));
    }

    [Fact]
    public async Task AwardAsync_CalledTwiceForTheSameTournament_DoesNotDuplicateTheHolder()
    {
        var championId = Guid.NewGuid();
        var tournament = CreateTournament(titleKinds: [TournamentTitleKind.Champion], participants: [(championId, 1)]);

        await _awarder.AwardAsync(tournament, []);
        await _unitOfWork.SaveChangesAsync();
        await _awarder.AwardAsync(tournament, []);
        await _unitOfWork.SaveChangesAsync();

        Assert.Equal(new[] { championId }, await HolderIdsAsync(tournament.Id, "Champion"));
    }

    private static FighterEntity Fighter(Guid heroId, int? hpLeft = null, int? cardsLeft = null)
        => new() { HeroId = heroId, HpLeft = hpLeft, CardsLeft = cardsLeft };

    private static MatchEntity OneVOneMatch(FighterEntity a, FighterEntity b)
        => new() { IsPlanned = false, Fighters = new List<FighterEntity> { a, b } };

    private static TournamentEntity CreateTournament(
        IEnumerable<TournamentTitleKind> titleKinds, IEnumerable<(Guid HeroId, int Placement)> participants)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Cup",
            CompletedAt = DateTime.UtcNow,
            TournamentTitles = titleKinds.Select(k => new TournamentTitleEntity { Kind = k }).ToList(),
            Participants = participants.Select(p => new TournamentParticipantEntity { HeroId = p.HeroId, FinalPlacement = p.Placement }).ToList()
        };

    private async Task<List<Guid>> HolderIdsAsync(Guid tournamentId, string nameContains)
    {
        var title = await _dbContext.Titles
            .Include(t => t.HeroTitles)
            .Where(t => t.TournamentId == tournamentId && t.Name.Contains(nameContains))
            .SingleAsync();
        return title.HeroTitles.Select(h => h.HeroesId).ToList();
    }
}
