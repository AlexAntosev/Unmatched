namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

public class TeamVsTeamMatchHandlerTests
{
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly TeamVsTeamMatchHandler _handler;

    public TeamVsTeamMatchHandlerTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);

        _handler = new TeamVsTeamMatchHandler(_unitOfWork.Object, new GameModeValidatorFactory(), new TeamVsTeamRatingCalculator());
    }

    [Fact]
    public async Task HandleAsync_ValidTwoVsTwoMatch_PersistsMatchWithoutTouchingRatings()
    {
        var fighters = new List<FighterEntity>
            {
                new() { HeroId = Guid.NewGuid(), Team = 1, IsWinner = true },
                new() { HeroId = Guid.NewGuid(), Team = 1, IsWinner = true },
                new() { HeroId = Guid.NewGuid(), Team = 2, IsWinner = false },
                new() { HeroId = Guid.NewGuid(), Team = 2, IsWinner = false }
            };
        var match = new MatchEntity { Fighters = fighters, GameMode = GameMode.TeamVsTeam };

        _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(match);

        await _handler.HandleAsync(match);

        Assert.All(fighters, f => Assert.Equal(0, f.MatchPoints));
        _ratingRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<RatingEntity>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
