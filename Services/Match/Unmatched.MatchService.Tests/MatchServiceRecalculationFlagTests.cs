namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Moq;

using Unmatched.MatchService.Contracts.Kafka;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Communication.Player;
using Unmatched.MatchService.Domain.Communication.Player.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Validation;

using Match = Unmatched.MatchService.Domain.Models.Match;
using Title = Unmatched.MatchService.Domain.Models.Title;

public class MatchServiceRecalculationFlagTests
{
    private static readonly Guid WinnerHeroId = Guid.NewGuid();
    private static readonly Guid LooserHeroId = Guid.NewGuid();

    private readonly Mock<IMatchHandler> _matchHandler = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IRatingRecalculationStateRepository> _recalculationStateRepository = new();
    private readonly Mock<ITournamentAwardRepository> _tournamentAwardRepository = new();

    private readonly MatchService _matchService;

    public MatchServiceRecalculationFlagTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _unitOfWork.Setup(u => u.RatingRecalculationState).Returns(_recalculationStateRepository.Object);
        _unitOfWork.Setup(u => u.TournamentAwards).Returns(_tournamentAwardRepository.Object);
        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(Array.Empty<TournamentAwardEntity>());

        _matchHandler.Setup(h => h.HandleAsync(It.IsAny<MatchEntity>())).Returns(Task.CompletedTask);

        var titleEvaluator = new TitleEvaluator(_unitOfWork.Object, _mapper.Object, []);

        var catalogHeroCache = new Mock<ICatalogHeroCache>();
        catalogHeroCache.Setup(c => c.GetAsync()).ReturnsAsync(new[]
        {
            new CatalogHeroDto { Id = WinnerHeroId, Name = "Winner Hero" },
            new CatalogHeroDto { Id = LooserHeroId, Name = "Looser Hero" },
        });

        var playerCache = new Mock<IPlayerCache>();
        playerCache.Setup(c => c.GetAsync()).ReturnsAsync(new[]
        {
            new PlayerDto { Id = Guid.Empty, Name = "Player" },
        });

        var kafkaProducer = new Mock<IKafkaProducer>();
        kafkaProducer.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<MatchCreated>())).Returns(Task.CompletedTask);

        _mapper
            .Setup(m => m.Map<MatchEntity>(It.IsAny<Match>()))
            .Returns((Match dto) => new MatchEntity
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                Date = dto.Date,
                Fighters = new List<FighterEntity>
                {
                    new() { HeroId = WinnerHeroId, IsWinner = true, MatchPoints = 0 },
                    new() { HeroId = LooserHeroId, IsWinner = false, MatchPoints = 0 },
                },
            });

        _mapper
            .Setup(m => m.Map<MatchCreated>(It.IsAny<MatchEntity>()))
            .Returns((MatchEntity e) => new MatchCreated
            {
                Id = e.Id,
                Date = e.Date,
                Fighters = e.Fighters.Select(f => new MatchCreated.Fighter { HeroId = f.HeroId, IsWinner = f.IsWinner }).ToList(),
            });

        _matchRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new MatchEntity
            {
                Id = id,
                Fighters = new List<FighterEntity>
                {
                    new() { HeroId = WinnerHeroId, IsWinner = true },
                    new() { HeroId = LooserHeroId, IsWinner = false },
                },
            });

        _matchService = new MatchService(
            _matchHandler.Object,
            new RankedMatchDataValidator(catalogHeroCache.Object),
            _mapper.Object,
            _unitOfWork.Object,
            titleEvaluator,
            catalogHeroCache.Object,
            playerCache.Object,
            kafkaProducer.Object);
    }

    [Fact]
    public async Task AddOrUpdateAsync_MatchDateEarlierThanExistingMatches_MarksRecalculationRequired()
    {
        var backdatedMatch = new Match { Id = Guid.NewGuid(), Date = new DateTime(2026, 1, 1), Fighters = Array.Empty<Domain.Models.Fighter>(), Comment = string.Empty };

        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity>
        {
            new() { Id = Guid.NewGuid(), Date = new DateTime(2026, 3, 1) },
            new() { Id = backdatedMatch.Id, Date = backdatedMatch.Date },
        });

        await _matchService.AddOrUpdateAsync(backdatedMatch);

        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(true), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_MatchDateIsTheLatest_DoesNotMarkRecalculationRequired()
    {
        var latestMatch = new Match { Id = Guid.NewGuid(), Date = new DateTime(2026, 3, 1), Fighters = Array.Empty<Domain.Models.Fighter>(), Comment = string.Empty };

        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity>
        {
            new() { Id = Guid.NewGuid(), Date = new DateTime(2026, 1, 1) },
            new() { Id = latestMatch.Id, Date = latestMatch.Date },
        });

        await _matchService.AddOrUpdateAsync(latestMatch);

        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task AddOrUpdateAsync_MatchDateEarlierThanExistingTournamentAward_MarksRecalculationRequired()
    {
        var backdatedMatch = new Match { Id = Guid.NewGuid(), Date = new DateTime(2026, 8, 1), Fighters = Array.Empty<Domain.Models.Fighter>(), Comment = string.Empty };

        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity>
        {
            new() { Id = Guid.NewGuid(), Date = new DateTime(2026, 7, 30) },
            new() { Id = backdatedMatch.Id, Date = backdatedMatch.Date },
        });

        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<TournamentAwardEntity>
        {
            new() { Id = Guid.NewGuid(), HeroId = WinnerHeroId, AwardedAt = new DateTime(2026, 8, 2), Points = 10 },
        });

        await _matchService.AddOrUpdateAsync(backdatedMatch);

        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(true), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_FirstMatchEver_DoesNotMarkRecalculationRequired()
    {
        var onlyMatch = new Match { Id = Guid.NewGuid(), Date = new DateTime(2026, 1, 1), Fighters = Array.Empty<Domain.Models.Fighter>(), Comment = string.Empty };

        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity>
        {
            new() { Id = onlyMatch.Id, Date = onlyMatch.Date },
        });

        await _matchService.AddOrUpdateAsync(onlyMatch);

        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(It.IsAny<bool>()), Times.Never);
    }
}
