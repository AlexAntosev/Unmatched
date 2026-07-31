namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Services;

public class RatingTimelineTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<ITournamentAwardRepository> _tournamentAwardRepository = new();

    private readonly RatingTimeline _timeline;

    public RatingTimelineTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.TournamentAwards).Returns(_tournamentAwardRepository.Object);

        _timeline = new RatingTimeline(_unitOfWork.Object);
    }

    [Fact]
    public async Task BuildAsync_MergesMatchesAndAwards_InChronologicalOrder_RegardlessOfInsertionOrder()
    {
        var matchFeb = new MatchEntity { Id = Guid.NewGuid(), Date = new DateTime(2026, 2, 1), Fighters = new List<FighterEntity>() };
        var matchApr = new MatchEntity { Id = Guid.NewGuid(), Date = new DateTime(2026, 4, 1), Fighters = new List<FighterEntity>() };
        var awardJan = new TournamentAwardEntity { Id = Guid.NewGuid(), AwardedAt = new DateTime(2026, 1, 1) };
        var awardMar = new TournamentAwardEntity { Id = Guid.NewGuid(), AwardedAt = new DateTime(2026, 3, 1) };

        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity> { matchApr, matchFeb });
        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<TournamentAwardEntity> { awardMar, awardJan });

        var timeline = await _timeline.BuildAsync();

        Assert.Equal(4, timeline.Count);
        Assert.Same(awardJan, timeline[0].Award);
        Assert.Same(matchFeb, timeline[1].Match);
        Assert.Same(awardMar, timeline[2].Award);
        Assert.Same(matchApr, timeline[3].Match);
    }

    [Fact]
    public async Task BuildAsync_EachEventCarriesExactlyOneOfMatchOrAward()
    {
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync())
            .ReturnsAsync(new List<MatchEntity> { new() { Id = Guid.NewGuid(), Date = new DateTime(2026, 1, 1), Fighters = new List<FighterEntity>() } });
        _tournamentAwardRepository.Setup(r => r.GetAsync())
            .ReturnsAsync(new List<TournamentAwardEntity> { new() { Id = Guid.NewGuid(), AwardedAt = new DateTime(2026, 1, 2) } });

        var timeline = await _timeline.BuildAsync();

        var matchEvent = timeline.Single(e => e.Match is not null);
        Assert.Null(matchEvent.Award);
        var awardEvent = timeline.Single(e => e.Award is not null);
        Assert.Null(awardEvent.Match);
    }

    [Fact]
    public async Task BuildAsync_NoMatchesOrAwards_ReturnsEmptyTimeline()
    {
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>());
        _tournamentAwardRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<TournamentAwardEntity>());

        var timeline = await _timeline.BuildAsync();

        Assert.Empty(timeline);
    }
}
