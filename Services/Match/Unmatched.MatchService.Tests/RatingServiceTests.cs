namespace Unmatched.MatchService.Tests;

using AutoMapper;

using Moq;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Services;

using Fighter = Unmatched.MatchService.Domain.Models.Fighter;
using Match = Unmatched.MatchService.Domain.Models.Match;
using SaveMatchResult = Unmatched.MatchService.Domain.Models.SaveMatchResult;

public class RatingServiceTests
{
    private readonly Mock<IMatchService> _matchService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IFighterRepository> _fighterRepository = new();
    private readonly Mock<IRatingRecalculationStateRepository> _recalculationStateRepository = new();
    private readonly Mock<IMapper> _mapper = new();

    private readonly RatingService _ratingService;

    public RatingServiceTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _unitOfWork.Setup(u => u.Fighters).Returns(_fighterRepository.Object);
        _unitOfWork.Setup(u => u.RatingRecalculationState).Returns(_recalculationStateRepository.Object);

        _mapper
            .Setup(m => m.Map<Match>(It.IsAny<MatchEntity>()))
            .Returns((MatchEntity e) => new Match { Id = e.Id, Date = e.Date, Fighters = Array.Empty<Fighter>(), Comment = string.Empty });

        _ratingService = new RatingService(_matchService.Object, _unitOfWork.Object, _mapper.Object);
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
        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity> { matchFeb, matchMar, matchJan });

        var replayOrder = new List<Guid>();
        _matchService
            .Setup(s => s.AddOrUpdateAsync(It.IsAny<Match>()))
            .Callback((Match m) => replayOrder.Add(m.Id))
            .ReturnsAsync((SaveMatchResult)null!);

        await _ratingService.RecalculateAsync();

        Assert.Equal(new[] { matchJan.Id, matchFeb.Id, matchMar.Id }, replayOrder);
    }

    [Fact]
    public async Task RecalculateAsync_ClearsExistingMatchesFightersAndRatingsBeforeReplaying()
    {
        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity>());

        await _ratingService.RecalculateAsync();

        _matchRepository.Verify(r => r.DeleteAll(), Times.Once);
        _fighterRepository.Verify(r => r.DeleteAll(), Times.Once);
        _ratingRepository.Verify(r => r.DeleteAll(), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RecalculateAsync_ClearsRecalculationRequiredFlagAfterReplaying()
    {
        _matchRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<MatchEntity>());

        await _ratingService.RecalculateAsync();

        _recalculationStateRepository.Verify(r => r.SetRecalculationRequiredAsync(false), Times.Once);
    }

    [Fact]
    public async Task IsRecalculationRequiredAsync_DelegatesToRepository()
    {
        _recalculationStateRepository.Setup(r => r.IsRecalculationRequiredAsync()).ReturnsAsync(true);

        var result = await _ratingService.IsRecalculationRequiredAsync();

        Assert.True(result);
    }
}
