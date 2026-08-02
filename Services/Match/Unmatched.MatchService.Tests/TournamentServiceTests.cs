namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Communication.Player;
using Unmatched.MatchService.Domain.Communication.Player.Dto;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Mapping;
using Unmatched.MatchService.Domain.Models;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Tournaments;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

/// <summary>Runs TournamentService against a real EF Core context (in-memory provider), following the
/// same pattern as RatingRecalculationPersistenceTests/TitleServiceTests - the child-collection
/// persistence (Participants/TournamentTitles) is exactly the kind of thing that looks fine against
/// mocks and silently breaks against a real change tracker.</summary>
public class TournamentServiceTests : IDisposable
{
    private static readonly Guid MapId = Guid.NewGuid();

    private readonly UnmatchedDbContext _dbContext;
    private readonly TournamentService _tournamentService;
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    public TournamentServiceTests()
    {
        var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new UnmatchedDbContext(options);

        var unitOfWork = new UnitOfWork(_dbContext);
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<DomainMapper>(), new LoggerFactory()).CreateMapper();

        var catalogMapCache = new Mock<ICatalogMapCache>();
        catalogMapCache.Setup(c => c.GetAsync()).ReturnsAsync(new[] { new CatalogMapDto { Id = MapId, Name = "Castle" } });

        var playerCache = new Mock<IPlayerCache>();
        playerCache.Setup(c => c.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new PlayerDto { Id = id, Name = id.ToString() });

        _tournamentService = new TournamentService(
            unitOfWork,
            mapper,
            _catalogHeroCache.Object,
            catalogMapCache.Object,
            playerCache.Object,
            new TournamentFormatGeneratorFactory(),
            new TournamentAwardScheduler(),
            new TournamentTitleAwarder(unitOfWork, _catalogHeroCache.Object));
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task AddAsync_PersistsParticipantsAsDraft()
    {
        var heroIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var created = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Test Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 4,
            InitialStage = Stage.SemiFinals,
            ParticipantHeroIds = heroIds
        });

        Assert.Equal(TournamentStatus.Draft, created.Status);
        Assert.Equal(heroIds.OrderBy(id => id), created.ParticipantHeroIds.OrderBy(id => id));
    }

    [Fact]
    public async Task AddAsync_AlwaysIncludesChampion_EvenWhenTheRequestOmitsIt()
    {
        var created = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Test Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            ParticipantHeroIds = [Guid.NewGuid(), Guid.NewGuid()],
            TitleKinds = [TournamentTitleKind.RunnerUp] // deliberately no Champion
        });

        Assert.Contains(TournamentTitleKind.Champion, created.TitleKinds);
        Assert.Contains(TournamentTitleKind.RunnerUp, created.TitleKinds);
    }

    [Fact]
    public async Task AddAsync_DoesNotDuplicateChampion_WhenTheRequestAlreadyIncludesIt()
    {
        var created = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Test Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            ParticipantHeroIds = [Guid.NewGuid(), Guid.NewGuid()],
            TitleKinds = [TournamentTitleKind.Champion, TournamentTitleKind.RunnerUp]
        });

        Assert.Single(created.TitleKinds, kind => kind == TournamentTitleKind.Champion);
    }

    [Fact]
    public async Task CreateNextStagePlannedMatchesAsync_FirstRound_DrawsOnlyFromTournamentParticipants()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        foreach (var heroId in participantIds)
        {
            SetUpHero(heroId);
        }

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Bracket Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 4,
            InitialStage = Stage.SemiFinals,
            ParticipantHeroIds = participantIds
        });

        await _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id);

        var matches = await _dbContext.Matches.Include(m => m.Fighters).Where(m => m.TournamentId == tournament.Id).ToListAsync();
        Assert.Equal(2, matches.Count);
        var fightersHeroIds = matches.SelectMany(m => m.Fighters).Select(f => f.HeroId).ToList();
        Assert.Equal(4, fightersHeroIds.Distinct().Count());
        Assert.All(fightersHeroIds, heroId => Assert.Contains(heroId, participantIds));
    }

    [Fact]
    public async Task CreateNextStagePlannedMatchesAsync_FlipsStatusToInProgress()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        foreach (var heroId in participantIds)
        {
            SetUpHero(heroId);
        }

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Duel Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            InitialStage = Stage.GrandFinals,
            ParticipantHeroIds = participantIds
        });

        await _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id);

        var reloaded = await _tournamentService.GetAsync(tournament.Id);
        Assert.Equal(TournamentStatus.InProgress, reloaded.Status);
    }

    [Fact]
    public async Task CreateNextStagePlannedMatchesAsync_CurrentStageStillPlanned_Throws()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        foreach (var heroId in participantIds)
        {
            SetUpHero(heroId);
        }

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Duel Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            InitialStage = Stage.GrandFinals,
            ParticipantHeroIds = participantIds
        });

        await _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id));
    }

    [Fact]
    public async Task CreateNextStagePlannedMatchesAsync_LeagueFormat_ThrowsBecauseItHasNoGenerator()
    {
        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Season",
            Format = TournamentFormat.League,
            MaxParticipants = 2,
            ParticipantHeroIds = [Guid.NewGuid(), Guid.NewGuid()]
        });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id));
    }

    [Fact]
    public async Task UpdateImageAsync_PersistsTheFileName()
    {
        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Art Cup",
            Format = TournamentFormat.League,
            MaxParticipants = 2,
            ParticipantHeroIds = [Guid.NewGuid(), Guid.NewGuid()]
        });

        var updated = await _tournamentService.UpdateImageAsync(tournament.Id, "cover.png");

        Assert.Equal("cover.png", updated!.ImageFileName);
        var reloaded = await _tournamentService.GetAsync(tournament.Id);
        Assert.Equal("cover.png", reloaded.ImageFileName);
    }

    [Fact]
    public async Task UpdateTrophyImageAsync_PersistsTheFileName()
    {
        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Art Cup",
            Format = TournamentFormat.League,
            MaxParticipants = 2,
            ParticipantHeroIds = [Guid.NewGuid(), Guid.NewGuid()]
        });

        var updated = await _tournamentService.UpdateTrophyImageAsync(tournament.Id, "trophy.png");

        Assert.Equal("trophy.png", updated!.TrophyImageFileName);
    }

    [Fact]
    public async Task GetStandingsAsync_CountsWinsAndLosses()
    {
        var winnerId = Guid.NewGuid();
        var loserId = Guid.NewGuid();

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Standings Cup",
            Format = TournamentFormat.League,
            MaxParticipants = 2,
            ParticipantHeroIds = [winnerId, loserId]
        });

        _dbContext.Matches.Add(new MatchEntity
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            GameMode = GameMode.OneVsOne,
            IsPlanned = false,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerId, IsWinner = true },
                new() { HeroId = loserId, IsWinner = false },
            },
        });
        await _dbContext.SaveChangesAsync();

        var standings = (await _tournamentService.GetStandingsAsync(tournament.Id)).ToList();

        Assert.Equal(1, standings.Single(s => s.HeroId == winnerId).Wins);
        Assert.Equal(0, standings.Single(s => s.HeroId == winnerId).Losses);
        Assert.Equal(1, standings.Single(s => s.HeroId == loserId).Losses);
    }

    [Fact]
    public async Task CompleteAsync_WritesAwardsAndPlacements_FlipsStatus_AndAppliesPointsToRatings()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        foreach (var heroId in participantIds)
        {
            SetUpHero(heroId);
        }

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Duel Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            InitialStage = Stage.GrandFinals,
            ParticipantHeroIds = participantIds
        });
        await _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id);
        await FinishMatchesAsync(tournament.Id, winnerHeroId: participantIds[0]);

        var completed = await _tournamentService.CompleteAsync(tournament.Id);

        Assert.Equal(TournamentStatus.Completed, completed.Status);
        Assert.NotNull(completed.CompletedAt);

        var awards = (await _tournamentService.GetAwardsAsync(tournament.Id)).ToList();
        Assert.Equal(0, awards.Sum(a => a.Points));
        Assert.Contains(awards, a => a.HeroId == participantIds[0] && a.AwardKind == TournamentAwardKind.Winner);

        var winnerRating = await _dbContext.Ratings.SingleAsync(r => r.HeroId == participantIds[0]);
        var winnerAwardPoints = awards.Where(a => a.HeroId == participantIds[0]).Sum(a => a.Points);
        Assert.Equal(RatingConstants.InitialRating + winnerAwardPoints, winnerRating.Points);

        var participants = await _dbContext.TournamentParticipants.Where(p => p.TournamentId == tournament.Id).ToListAsync();
        Assert.Equal(1, participants.Single(p => p.HeroId == participantIds[0]).FinalPlacement);
        Assert.Equal(2, participants.Single(p => p.HeroId == participantIds[1]).FinalPlacement);
    }

    [Fact]
    public async Task CompleteAsync_CalledTwice_ThrowsAndDoesNotDuplicateAwards()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        foreach (var heroId in participantIds)
        {
            SetUpHero(heroId);
        }

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Duel Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            InitialStage = Stage.GrandFinals,
            ParticipantHeroIds = participantIds
        });
        await _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id);
        await FinishMatchesAsync(tournament.Id, winnerHeroId: participantIds[0]);
        await _tournamentService.CompleteAsync(tournament.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _tournamentService.CompleteAsync(tournament.Id));

        var awards = await _tournamentService.GetAwardsAsync(tournament.Id);
        Assert.Equal(2, awards.Count());
    }

    [Fact]
    public async Task CompleteAsync_PlannedMatchesRemain_Throws()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        foreach (var heroId in participantIds)
        {
            SetUpHero(heroId);
        }

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Duel Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            InitialStage = Stage.GrandFinals,
            ParticipantHeroIds = participantIds
        });
        await _tournamentService.CreateNextStagePlannedMatchesAsync(tournament.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _tournamentService.CompleteAsync(tournament.Id));
    }

    private async Task FinishMatchesAsync(Guid tournamentId, Guid winnerHeroId)
    {
        var matches = await _dbContext.Matches.Include(m => m.Fighters).Where(m => m.TournamentId == tournamentId).ToListAsync();
        foreach (var match in matches)
        {
            match.IsPlanned = false;
            foreach (var fighter in match.Fighters)
            {
                fighter.IsWinner = fighter.HeroId == winnerHeroId;
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private void SetUpHero(Guid heroId)
        => _catalogHeroCache
            .Setup(c => c.GetAsync(heroId))
            .ReturnsAsync(new CatalogHeroDto { Id = heroId, Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() });
}
