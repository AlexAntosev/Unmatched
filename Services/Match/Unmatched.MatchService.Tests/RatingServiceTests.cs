namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Moq;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Services;
using Unmatched.MatchService.Domain.Titles;

public class RatingServiceTests
{
    private readonly Mock<IMatchHandler> _matchHandler = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IFighterRepository> _fighterRepository = new();
    private readonly Mock<IRatingRecalculationStateRepository> _recalculationStateRepository = new();
    private readonly Mock<ITournamentAwardRepository> _tournamentAwardRepository = new();
    private readonly Mock<IHeroTitleRepository> _heroTitleRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly RatingService _ratingService;

    public RatingServiceTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _unitOfWork.Setup(u => u.Fighters).Returns(_fighterRepository.Object);
        _unitOfWork.Setup(u => u.RatingRecalculationState).Returns(_recalculationStateRepository.Object);
        _unitOfWork.Setup(u => u.TournamentAwards).Returns(_tournamentAwardRepository.Object);
        _unitOfWork.Setup(u => u.HeroTitles).Returns(_heroTitleRepository.Object);
        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<TournamentAwardEntity>());
        _tournamentAwardRepository.Setup(r => r.DeleteByAwardKindAsync(It.IsAny<TournamentAwardKind>())).Returns(Task.CompletedTask);

        _matchHandler.Setup(h => h.HandleAsync(It.IsAny<MatchEntity>())).Returns(Task.CompletedTask);

        var titleEvaluator = new TitleEvaluator(_unitOfWork.Object, _mapper.Object, []);
        var titleAwarder = new TournamentTitleAwarder(_unitOfWork.Object, new Mock<Domain.Communication.Catalog.ICatalogHeroCache>().Object);

        _ratingService = new RatingService(
            _matchHandler.Object,
            _unitOfWork.Object,
            _mapper.Object,
            new RatingTimeline(_unitOfWork.Object),
            titleEvaluator,
            titleAwarder);
    }

    [Fact]
    public async Task RecalculateAsync_ReplaysMatchesInChronologicalOrder_RegardlessOfHowStorageReturnsThem()
    {
        // this is the exact scenario a "backdated" match creates: it's inserted/stored after matches with a
        // later Date, so a naive replay (in storage/insertion order) would re-derive ratings against the wrong
        // history. RecalculateAsync must re-sequence by Date before replaying, since each match's handicap
        // depends on the hero ratings accumulated from every match *before* it, not on insertion order.
        var matchJan = new MatchEntity { Id = Guid.NewGuid(), Date = new DateTime(2026, 1, 1), Fighters = new List<FighterEntity>() };
        var matchFeb = new MatchEntity { Id = Guid.NewGuid(), Date = new DateTime(2026, 2, 1), Fighters = new List<FighterEntity>() };
        var matchMar = new MatchEntity { Id = Guid.NewGuid(), Date = new DateTime(2026, 3, 1), Fighters = new List<FighterEntity>() };

        // storage returns them out of chronological order - as if matchJan was the backdated one, added last
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity> { matchFeb, matchMar, matchJan });

        var replayOrder = new List<Guid>();
        _matchHandler
            .Setup(h => h.HandleAsync(It.IsAny<MatchEntity>()))
            .Callback((MatchEntity m) => replayOrder.Add(m.Id))
            .Returns(Task.CompletedTask);

        await _ratingService.RecalculateAsync();

        Assert.Equal(new[] { matchJan.Id, matchFeb.Id, matchMar.Id }, replayOrder);
    }

    [Fact]
    public async Task RecalculateAsync_ClearsRatings_ButNeverDeletesMatchesOrFighters()
    {
        // ratings are derived data and get rebuilt by the replay; the match history is the source of truth
        // and has to survive - deleting it up front meant a failing replay wiped it permanently.
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>());

        await _ratingService.RecalculateAsync();

        _ratingRepository.Verify(r => r.DeleteAll(), Times.Once);
        _matchRepository.Verify(r => r.DeleteAll(), Times.Never);
        _fighterRepository.Verify(r => r.DeleteAll(), Times.Never);
    }

    [Fact]
    public async Task RecalculateAsync_ReplaysOnlyFinishedMatches()
    {
        // a planned match has no result to score, and replaying one would silently mark it played
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>());

        await _ratingService.RecalculateAsync();

        _matchRepository.Verify(r => r.GetFinishedForRatingReplayAsync(), Times.Once);
        _matchRepository.Verify(r => r.GetAsync(), Times.Never);
    }

    [Fact]
    public async Task RecalculateAsync_ClearsRecalculationRequiredFlagAfterReplaying()
    {
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>());

        await _ratingService.RecalculateAsync();

        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(false), Times.Once);
    }

    [Fact]
    public async Task RecalculateAsync_ReplayFails_LeavesRecalculationRequiredFlagRaised()
    {
        var match = new MatchEntity { Id = Guid.NewGuid(), Date = new DateTime(2026, 1, 1), Fighters = new List<FighterEntity>() };
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity> { match });
        _matchHandler.Setup(h => h.HandleAsync(It.IsAny<MatchEntity>())).ThrowsAsync(new InvalidOperationException("boom"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _ratingService.RecalculateAsync());

        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(true), Times.Once);
        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(false), Times.Never);
    }

    [Fact]
    public async Task GetRatingChangesAsync_WalksForwardFromInitialRating_IncludingAwards()
    {
        var heroId = Guid.NewGuid();
        var matchJan = new MatchEntity
        {
            Id = Guid.NewGuid(),
            Date = new DateTime(2026, 1, 1),
            Fighters = new List<FighterEntity> { new() { HeroId = heroId, MatchPoints = 20, IsWinner = true } }
        };
        var award = new TournamentAwardEntity { Id = Guid.NewGuid(), HeroId = heroId, Points = 50, AwardedAt = new DateTime(2026, 2, 1) };

        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity> { matchJan });
        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<TournamentAwardEntity> { award });

        var changes = await _ratingService.GetRatingChangesAsync(heroId);

        Assert.Equal(2, changes.Count);
        Assert.Equal(RatingConstants.InitialRating + 20, changes[0].RatingDelta);
        Assert.Equal(RatingConstants.InitialRating + 20 + 50, changes[1].RatingDelta);
    }

    [Fact]
    public async Task GetRatingChangesAsync_ExcludesMatchesAndAwardsForOtherHeroes()
    {
        var heroId = Guid.NewGuid();
        var otherHeroMatch = new MatchEntity
        {
            Id = Guid.NewGuid(),
            Date = new DateTime(2026, 1, 1),
            Fighters = new List<FighterEntity> { new() { HeroId = Guid.NewGuid(), MatchPoints = 20, IsWinner = true } }
        };
        var otherHeroAward = new TournamentAwardEntity { Id = Guid.NewGuid(), HeroId = Guid.NewGuid(), Points = 50, AwardedAt = new DateTime(2026, 2, 1) };

        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity> { otherHeroMatch });
        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<TournamentAwardEntity> { otherHeroAward });

        var changes = await _ratingService.GetRatingChangesAsync(heroId);

        Assert.Empty(changes);
    }

    [Fact]
    public async Task IsRecalculationRequiredAsync_DelegatesToRepository()
    {
        _recalculationStateRepository.Setup(r => r.IsRecalculationRequiredAsync()).ReturnsAsync(true);

        var result = await _ratingService.IsRecalculationRequiredAsync();

        Assert.True(result);
    }
}
