namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Moq;

using Unmatched.MatchService.Contracts.Kafka;
using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Communication.Player;
using Unmatched.MatchService.Domain.Communication.Player.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.Domain.Validation;

using Match = Unmatched.MatchService.Domain.Models.Match;

/// <summary>Covers the two pieces of AddOrUpdateAsync's result-building that used to be discarded before
/// reaching the caller: each fighter's rating movement (captured around the handler call, since it's the
/// handler that actually applies the rating change) and per-fighter title attribution (TitleEvaluator
/// already knows which hero newly earned what - this just checks MatchService routes it onto the right
/// FighterResult instead of flattening it). The handler itself is a no-op mock, same as
/// MatchServiceRecalculationFlagTests - the rating math belongs to OneVsOneRatingCalculatorTests etc.</summary>
public class MatchServiceResultTests
{
    private static readonly Guid WinnerHeroId = Guid.NewGuid();
    private static readonly Guid LooserHeroId = Guid.NewGuid();

    private readonly Mock<IMatchHandler> _matchHandler = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<ITitleRepository> _titleRepository = new();
    private readonly Mock<IRatingRecalculationStateRepository> _recalculationStateRepository = new();
    private readonly Mock<ITournamentAwardRepository> _tournamentAwardRepository = new();

    private readonly MatchService _matchService;

    public MatchServiceResultTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _unitOfWork.Setup(u => u.Titles).Returns(_titleRepository.Object);
        _unitOfWork.Setup(u => u.RatingRecalculationState).Returns(_recalculationStateRepository.Object);
        _unitOfWork.Setup(u => u.TournamentAwards).Returns(_tournamentAwardRepository.Object);
        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(Array.Empty<TournamentAwardEntity>());
        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity>());
        _titleRepository.Setup(r => r.GetByRuleKeyAsync(It.IsAny<string>())).ReturnsAsync((TitleEntity?)null);

        _matchHandler.Setup(h => h.HandleAsync(It.IsAny<MatchEntity>())).Returns(Task.CompletedTask);

        var titleEvaluator = new TitleEvaluator(_unitOfWork.Object, []);

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
                GameMode = Domain.Enums.GameMode.OneVsOne,
                IsRanked = dto.IsRanked,
                Fighters = new List<FighterEntity>
                {
                    new() { HeroId = WinnerHeroId, IsWinner = true, MatchPoints = 18, HpLeft = 10, CardsLeft = 5 },
                    new() { HeroId = LooserHeroId, IsWinner = false, MatchPoints = -18, HpLeft = 0, CardsLeft = 3 },
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
                GameMode = Domain.Enums.GameMode.OneVsOne,
                Fighters = new List<FighterEntity>
                {
                    new() { HeroId = WinnerHeroId, IsWinner = true, MatchPoints = 18, HpLeft = 10, CardsLeft = 5 },
                    new() { HeroId = LooserHeroId, IsWinner = false, MatchPoints = -18, HpLeft = 0, CardsLeft = 3 },
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

    private static Match RankedMatch(bool isRanked = true) => new()
    {
        Id = Guid.NewGuid(),
        Date = new DateTime(2026, 7, 1),
        Comment = string.Empty,
        IsRanked = isRanked,
        Fighters = Array.Empty<Domain.Models.Fighter>(),
    };

    [Fact]
    public async Task AddOrUpdateAsync_RankedMatch_ReportsRatingBeforeAndAfterPerFighter()
    {
        _ratingRepository.SetupSequence(r => r.GetByHeroIdAsync(WinnerHeroId))
            .ReturnsAsync(new RatingEntity { HeroId = WinnerHeroId, Points = 1824 })
            .ReturnsAsync(new RatingEntity { HeroId = WinnerHeroId, Points = 1842 });
        _ratingRepository.SetupSequence(r => r.GetByHeroIdAsync(LooserHeroId))
            .ReturnsAsync(new RatingEntity { HeroId = LooserHeroId, Points = 1795 })
            .ReturnsAsync(new RatingEntity { HeroId = LooserHeroId, Points = 1777 });

        var result = await _matchService.AddOrUpdateAsync(RankedMatch());

        var winner = result.FighterResults.Single(f => f.HeroName == "Winner Hero");
        var looser = result.FighterResults.Single(f => f.HeroName == "Looser Hero");
        Assert.Equal(1824, winner.RatingBefore);
        Assert.Equal(1842, winner.RatingAfter);
        Assert.Equal(1795, looser.RatingBefore);
        Assert.Equal(1777, looser.RatingAfter);
    }

    [Fact]
    public async Task AddOrUpdateAsync_UnrankedMatch_LeavesRatingFieldsNull()
    {
        var result = await _matchService.AddOrUpdateAsync(RankedMatch(isRanked: false));

        Assert.All(result.FighterResults, f => Assert.Null(f.RatingBefore));
        Assert.All(result.FighterResults, f => Assert.Null(f.RatingAfter));
        _ratingRepository.Verify(r => r.GetByHeroIdAsync(It.IsAny<Guid>()), Times.Exactly(2));
    }

    [Fact]
    public async Task AddOrUpdateAsync_HeroNewlyEarnsATitle_AttachesItToThatFightersResultOnly()
    {
        var title = new TitleEntity { Id = Guid.NewGuid(), Name = "The Streak", RuleKey = "streak", Exclusivity = TitleExclusivity.Unique, HeroTitles = new List<HeroTitleEntity>() };
        _titleRepository.Setup(r => r.GetByRuleKeyAsync("streak")).ReturnsAsync(title);
        var streakRule = new StubTitleRule("streak", TitleExclusivity.Unique, WinnerHeroId);
        var titleEvaluator = new TitleEvaluator(_unitOfWork.Object, new ITitleRule[] { streakRule });
        var matchService = new MatchService(
            _matchHandler.Object,
            new RankedMatchDataValidator(new Mock<ICatalogHeroCache>().Object),
            _mapper.Object,
            _unitOfWork.Object,
            titleEvaluator,
            MockCatalogHeroCache(),
            MockPlayerCache(),
            new Mock<IKafkaProducer>().Object);

        var result = await matchService.AddOrUpdateAsync(RankedMatch());

        var winner = result.FighterResults.Single(f => f.HeroName == "Winner Hero");
        var looser = result.FighterResults.Single(f => f.HeroName == "Looser Hero");
        var earned = Assert.Single(winner.EarnedTitles);
        Assert.Equal("streak", earned.RuleKey);
        Assert.Equal("The Streak", earned.Name);
        Assert.Empty(looser.EarnedTitles);
    }

    private ICatalogHeroCache MockCatalogHeroCache()
    {
        var mock = new Mock<ICatalogHeroCache>();
        mock.Setup(c => c.GetAsync()).ReturnsAsync(new[]
        {
            new CatalogHeroDto { Id = WinnerHeroId, Name = "Winner Hero" },
            new CatalogHeroDto { Id = LooserHeroId, Name = "Looser Hero" },
        });
        return mock.Object;
    }

    private IPlayerCache MockPlayerCache()
    {
        var mock = new Mock<IPlayerCache>();
        mock.Setup(c => c.GetAsync()).ReturnsAsync(new[] { new PlayerDto { Id = Guid.Empty, Name = "Player" } });
        return mock.Object;
    }

    private class StubTitleRule(string ruleKey, TitleExclusivity exclusivity, params Guid[] qualifiers) : ITitleRule
    {
        public string RuleKey { get; } = ruleKey;

        public TitleExclusivity Exclusivity { get; } = exclusivity;

        public Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
            => Task.FromResult<IReadOnlyDictionary<Guid, double?>>(qualifiers.ToDictionary(id => id, _ => (double?)null));
    }
}
