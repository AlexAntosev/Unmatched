namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

public class MatchHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IGameModeValidatorFactory> _validatorFactory = new();
    private readonly Mock<IGameModeValidator> _validator = new();
    private readonly Mock<IRatingCalculatorFactory> _ratingCalculatorFactory = new();
    private readonly Mock<IRatingCalculator> _ratingCalculator = new();

    private readonly MatchHandler _handler;

    public MatchHandlerTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _validatorFactory.Setup(f => f.Create(It.IsAny<GameMode>())).Returns(_validator.Object);
        _ratingCalculatorFactory.Setup(f => f.Create(It.IsAny<GameMode>())).Returns(_ratingCalculator.Object);

        _handler = new MatchHandler(_unitOfWork.Object, _validatorFactory.Object, _ratingCalculatorFactory.Object);
    }

    [Fact]
    public async Task HandleAsync_DelegatesValidationToTheGameModeValidator()
    {
        var match = CreateMatch(GameMode.OneVsOne);

        await _handler.HandleAsync(match);

        _validatorFactory.Verify(f => f.Create(GameMode.OneVsOne), Times.Once);
        _validator.Verify(v => v.Validate(match), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ValidationFails_PropagatesAndNeverConsultsTheCalculatorOrSaves()
    {
        var match = CreateMatch(GameMode.OneVsOne);
        _validator.Setup(v => v.Validate(match)).Throws(new ArgumentException("bad match"));

        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(match));

        _ratingCalculatorFactory.Verify(f => f.Create(It.IsAny<GameMode>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Unranked_NeverConsultsTheCalculator_AndEveryFighterScoresZero()
    {
        var winnerId = Guid.NewGuid();
        var looserId = Guid.NewGuid();
        var match = CreateMatch(GameMode.OneVsOne, isRanked: false, winnerId, looserId);

        await _handler.HandleAsync(match);

        _ratingCalculatorFactory.Verify(f => f.Create(It.IsAny<GameMode>()), Times.Never);
        Assert.All(match.Fighters, f => Assert.Equal(0, f.MatchPoints));
        _ratingRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<RatingEntity>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Ranked_AssignsMatchPointsFromTheCalculatorResult()
    {
        var winnerId = Guid.NewGuid();
        var looserId = Guid.NewGuid();
        var match = CreateMatch(GameMode.OneVsOne, isRanked: true, winnerId, looserId);
        _ratingCalculator.Setup(c => c.CalculateAsync(match)).ReturnsAsync(new Dictionary<Guid, int> { [winnerId] = 16, [looserId] = -16 });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(It.IsAny<Guid>())).ReturnsAsync((RatingEntity?)null);

        await _handler.HandleAsync(match);

        Assert.Equal(16, match.Fighters.Single(f => f.HeroId == winnerId).MatchPoints);
        Assert.Equal(-16, match.Fighters.Single(f => f.HeroId == looserId).MatchPoints);
    }

    [Fact]
    public async Task HandleAsync_RankedFighterMissingFromTheCalculatorResult_ScoresZero()
    {
        // e.g. a hero on the losing side of a mode whose calculator only scored the winner
        var heroId = Guid.NewGuid();
        var match = CreateMatch(GameMode.OneVsOne, isRanked: true, heroId, Guid.NewGuid());
        _ratingCalculator.Setup(c => c.CalculateAsync(match)).ReturnsAsync(new Dictionary<Guid, int>());

        await _handler.HandleAsync(match);

        Assert.All(match.Fighters, f => Assert.Equal(0, f.MatchPoints));
    }

    [Fact]
    public async Task HandleAsync_HeroWithNoExistingRatingRow_StartsAtInitialRatingBeforeApplyingTheDelta()
    {
        var heroId = Guid.NewGuid();
        var match = CreateMatch(GameMode.OneVsOne, isRanked: true, heroId, Guid.NewGuid());
        _ratingCalculator.Setup(c => c.CalculateAsync(match)).ReturnsAsync(new Dictionary<Guid, int> { [heroId] = 20 });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(It.IsAny<Guid>())).ReturnsAsync((RatingEntity?)null);

        RatingEntity? saved = null;
        _ratingRepository
            .Setup(r => r.AddOrUpdateAsync(It.Is<RatingEntity>(r => r.HeroId == heroId)))
            .Callback((RatingEntity rating) => saved = rating)
            .Returns(Task.CompletedTask);

        await _handler.HandleAsync(match);

        Assert.Equal(RatingConstants.InitialRating + 20, saved!.Points);
    }

    [Fact]
    public async Task HandleAsync_ExistingRating_AddsMatchPointsToWhatWasAlreadyThere()
    {
        var heroId = Guid.NewGuid();
        var match = CreateMatch(GameMode.OneVsOne, isRanked: true, heroId, Guid.NewGuid());
        _ratingCalculator.Setup(c => c.CalculateAsync(match)).ReturnsAsync(new Dictionary<Guid, int> { [heroId] = 20 });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(heroId)).ReturnsAsync(new RatingEntity { HeroId = heroId, Points = 1050 });

        RatingEntity? saved = null;
        _ratingRepository
            .Setup(r => r.AddOrUpdateAsync(It.Is<RatingEntity>(r => r.HeroId == heroId)))
            .Callback((RatingEntity rating) => saved = rating)
            .Returns(Task.CompletedTask);

        await _handler.HandleAsync(match);

        Assert.Equal(1070, saved!.Points);
    }

    [Fact]
    public async Task HandleAsync_NewMatch_AddsRatherThanUpdates()
    {
        var match = CreateMatch(GameMode.OneVsOne);
        match.Id = Guid.Empty;

        await _handler.HandleAsync(match);

        _matchRepository.Verify(r => r.AddAsync(match), Times.Once);
        _matchRepository.Verify(r => r.Update(It.IsAny<MatchEntity>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ExistingMatch_UpdatesRatherThanAdds()
    {
        var match = CreateMatch(GameMode.OneVsOne);
        match.Id = Guid.NewGuid();

        await _handler.HandleAsync(match);

        _matchRepository.Verify(r => r.Update(match), Times.Once);
        _matchRepository.Verify(r => r.AddAsync(It.IsAny<MatchEntity>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_MarksTheMatchAsNoLongerPlanned()
    {
        var match = CreateMatch(GameMode.OneVsOne);
        match.IsPlanned = true;

        await _handler.HandleAsync(match);

        Assert.False(match.IsPlanned);
    }

    [Fact]
    public async Task HandleAsync_SavesChangesExactlyOnce()
    {
        var match = CreateMatch(GameMode.OneVsOne);

        await _handler.HandleAsync(match);

        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    private static MatchEntity CreateMatch(GameMode gameMode, bool isRanked = false, Guid? winnerId = null, Guid? looserId = null)
        => new()
        {
            GameMode = gameMode,
            IsRanked = isRanked,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerId ?? Guid.NewGuid(), IsWinner = true },
                new() { HeroId = looserId ?? Guid.NewGuid(), IsWinner = false },
            },
        };
}
