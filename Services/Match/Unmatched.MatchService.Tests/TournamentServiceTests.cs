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

    [Fact]
    public async Task RecomputeCompletionAwardsAsync_NotCompleted_Throws()
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

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tournamentService.RecomputeCompletionAwardsAsync(tournament.Id));
    }

    /// <summary>Mirrors the real backfill bug: a hero who never actually played the bracket ends up in
    /// TournamentParticipants anyway, so CompleteAsync's self-funded pool divides its entry fee among 3
    /// "participants" instead of the real 2 - the phantom hero is charged a bogus penalty it never earned,
    /// and the real finishers' payouts are computed against the wrong field size. Recomputing after the
    /// phantom row is removed (what the participant-correction migration does) must replace the stale
    /// awards with ones based on just the real bracket participants.</summary>
    [Fact]
    public async Task RecomputeCompletionAwardsAsync_AfterRemovingANonBracketParticipant_ReplacesTheStaleAwards()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var phantomHeroId = Guid.NewGuid();
        foreach (var heroId in participantIds.Append(phantomHeroId))
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

        // Simulates the real backfill bug directly at the data level: a hero who never played any match
        // in this tournament still ends up as a TournamentParticipants row (never through AddAsync/the
        // bracket generator, exactly like the historical backfill that swept in unrelated match data).
        _dbContext.TournamentParticipants.Add(new TournamentParticipantEntity
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            HeroId = phantomHeroId
        });
        await _dbContext.SaveChangesAsync();

        await _tournamentService.CompleteAsync(tournament.Id);

        var buggyAwards = (await _tournamentService.GetAwardsAsync(tournament.Id)).ToList();
        Assert.Equal(3, buggyAwards.Count);
        Assert.True(buggyAwards.Single(a => a.HeroId == phantomHeroId).Points < 0);

        var phantomParticipant = await _dbContext.TournamentParticipants
            .SingleAsync(p => p.TournamentId == tournament.Id && p.HeroId == phantomHeroId);
        _dbContext.TournamentParticipants.Remove(phantomParticipant);
        await _dbContext.SaveChangesAsync();

        await _tournamentService.RecomputeCompletionAwardsAsync(tournament.Id);

        var fixedAwards = (await _tournamentService.GetAwardsAsync(tournament.Id)).ToList();
        Assert.Equal(2, fixedAwards.Count);
        Assert.DoesNotContain(fixedAwards, a => a.HeroId == phantomHeroId);
        Assert.Equal(0, fixedAwards.Sum(a => a.Points));
        Assert.NotEqual(
            buggyAwards.Single(a => a.HeroId == participantIds[0]).Points,
            fixedAwards.Single(a => a.HeroId == participantIds[0]).Points);

        var remainingParticipants = await _dbContext.TournamentParticipants
            .Where(p => p.TournamentId == tournament.Id)
            .ToListAsync();
        Assert.Equal(1, remainingParticipants.Single(p => p.HeroId == participantIds[0]).FinalPlacement);
        Assert.Equal(2, remainingParticipants.Single(p => p.HeroId == participantIds[1]).FinalPlacement);
    }

    /// <summary>The startup self-heal: it must find and fix a completed tournament whose awards drifted
    /// from its participants (same scenario as the direct RecomputeCompletionAwardsAsync test above),
    /// without needing anyone to know that specific tournament's id.</summary>
    [Fact]
    public async Task ReconcileCompletionAwardsAsync_FixesATournamentWhoseAwardsDontMatchItsParticipants()
    {
        var participantIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var phantomHeroId = Guid.NewGuid();
        foreach (var heroId in participantIds.Append(phantomHeroId))
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

        _dbContext.TournamentParticipants.Add(new TournamentParticipantEntity
        {
            Id = Guid.NewGuid(),
            TournamentId = tournament.Id,
            HeroId = phantomHeroId
        });
        await _dbContext.SaveChangesAsync();
        await _tournamentService.CompleteAsync(tournament.Id);

        _dbContext.TournamentParticipants.Remove(await _dbContext.TournamentParticipants
            .SingleAsync(p => p.TournamentId == tournament.Id && p.HeroId == phantomHeroId));
        await _dbContext.SaveChangesAsync();

        await _tournamentService.ReconcileCompletionAwardsAsync();

        var awards = (await _tournamentService.GetAwardsAsync(tournament.Id)).ToList();
        Assert.Equal(2, awards.Count);
        Assert.DoesNotContain(awards, a => a.HeroId == phantomHeroId);
        Assert.Equal(0, awards.Sum(a => a.Points));
    }

    [Fact]
    public async Task ReconcileCompletionAwardsAsync_LeavesAnAlreadyCorrectTournamentsAwardsUntouched()
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

        var awardIdsBefore = (await _dbContext.TournamentAwards.Where(a => a.TournamentId == tournament.Id).ToListAsync())
            .Select(a => a.Id)
            .OrderBy(id => id)
            .ToList();

        await _tournamentService.ReconcileCompletionAwardsAsync();

        var awardIdsAfter = (await _dbContext.TournamentAwards.Where(a => a.TournamentId == tournament.Id).ToListAsync())
            .Select(a => a.Id)
            .OrderBy(id => id)
            .ToList();
        Assert.Equal(awardIdsBefore, awardIdsAfter);
    }

    [Fact]
    public async Task GetBountyStateAsync_NoMatchesYet_ChampionIsTheStartingChampion()
    {
        var startingChampionId = Guid.NewGuid();
        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Bounty Pool",
            Format = TournamentFormat.Bounty,
            StartingChampionId = startingChampionId
        });

        var state = await _tournamentService.GetBountyStateAsync(tournament.Id);

        Assert.Equal(startingChampionId, state.ChampionHeroId);
        Assert.Equal(0, state.DefenseCount);
    }

    [Fact]
    public async Task CreateBountyChallengeAsync_CreatesAPlannedOneVOneMatchAgainstTheCurrentChampion()
    {
        var championId = Guid.NewGuid();
        var challengerId = Guid.NewGuid();
        var championPlayerId = Guid.NewGuid();
        var challengerPlayerId = Guid.NewGuid();
        SetUpHero(championId);
        SetUpHero(challengerId);

        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Bounty Pool",
            Format = TournamentFormat.Bounty,
            StartingChampionId = championId
        });

        var match = await _tournamentService.CreateBountyChallengeAsync(
            tournament.Id, challengerId, championPlayerId, challengerPlayerId, MapId);

        Assert.True(match.IsPlanned);
        Assert.Null(match.Stage);
        Assert.Equal(tournament.Id, match.TournamentId);
        var fighterHeroIds = match.Fighters.Select(f => f.HeroId).ToList();
        Assert.Equal(new[] { championId, challengerId }.OrderBy(id => id), fighterHeroIds.OrderBy(id => id));
    }

    [Fact]
    public async Task CreateBountyChallengeAsync_ChallengerIsTheCurrentChampion_Throws()
    {
        var championId = Guid.NewGuid();
        SetUpHero(championId);
        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Bounty Pool",
            Format = TournamentFormat.Bounty,
            StartingChampionId = championId
        });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tournamentService.CreateBountyChallengeAsync(tournament.Id, championId, Guid.NewGuid(), Guid.NewGuid(), MapId));
    }

    [Fact]
    public async Task CreateBountyChallengeAsync_NonBountyTournament_Throws()
    {
        var tournament = await _tournamentService.AddAsync(new Tournament
        {
            Name = "Bracket Cup",
            Format = TournamentFormat.SingleElimination,
            MaxParticipants = 2,
            ParticipantHeroIds = [Guid.NewGuid(), Guid.NewGuid()]
        });

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _tournamentService.CreateBountyChallengeAsync(tournament.Id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), MapId));
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
