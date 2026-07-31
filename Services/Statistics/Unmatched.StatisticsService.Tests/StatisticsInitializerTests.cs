namespace Unmatched.StatisticsService.Tests;

using Microsoft.Extensions.Logging;

using Moq;

using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Initialize;
using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Repositories;

public class StatisticsInitializerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchClient> _matchClient = new();
    private readonly Mock<IStatsCoordinator> _coordinator = new();

    private readonly StatisticsInitializer _initializer;

    public StatisticsInitializerTests()
    {
        _initializer = new StatisticsInitializer(
            new Mock<ILogger<StatisticsInitializer>>().Object,
            _unitOfWork.Object,
            new[] { _coordinator.Object },
            _matchClient.Object);
    }

    [Fact]
    public async Task RebuildAsync_RebuildsEveryCoordinator_RegardlessOfWhetherItAlreadyHasData()
    {
        // RebuildAsync exists precisely so a rating recalculation (which never publishes the
        // match-created event these read models are otherwise kept in sync by) can force every
        // coordinator to rebuild - checking HasDataAsync first would skip exactly the coordinators
        // that already have (now stale) data.
        _matchClient.Setup(c => c.GetAllMatchesAsync()).ReturnsAsync(Array.Empty<MatchDto>());

        await _initializer.RebuildAsync();

        _coordinator.Verify(c => c.InitializeAsync(It.IsAny<IReadOnlyCollection<MatchDto>>()), Times.Once);
        _coordinator.Verify(c => c.HasDataAsync(), Times.Never);
        _coordinator.Verify(c => c.CheckAndInitializeAsync(), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RebuildAsync_ExcludesPlannedMatches_FromEveryCoordinator()
    {
        var (matches, passedMatches) = await CaptureMatchesPassedToCoordinatorAsync(_initializer.RebuildAsync);

        Assert.Single(passedMatches, m => m.Id == matches.Finished.Id);
    }

    [Fact]
    public async Task InitializeAsync_NewCoordinator_ReceivesMatchesWithPlannedOnesExcluded()
    {
        _coordinator.Setup(c => c.HasDataAsync()).ReturnsAsync(false);

        var (matches, passedMatches) = await CaptureMatchesPassedToCoordinatorAsync(_initializer.InitializeAsync);

        Assert.Single(passedMatches, m => m.Id == matches.Finished.Id);
    }

    /// <summary>Seeds a finished and a planned match, runs <paramref name="action"/>, and captures what
    /// the coordinator's InitializeAsync actually received - the one thing both InitializeAsync's
    /// "first run" path and RebuildAsync are supposed to agree on.</summary>
    private async Task<((MatchDto Finished, MatchDto Planned) Matches, IReadOnlyCollection<MatchDto> Passed)> CaptureMatchesPassedToCoordinatorAsync(Func<Task> action)
    {
        var finished = new MatchDto { Id = Guid.NewGuid(), IsPlanned = false, Fighters = Array.Empty<FighterDto>() };
        var planned = new MatchDto { Id = Guid.NewGuid(), IsPlanned = true, Fighters = Array.Empty<FighterDto>() };
        _matchClient.Setup(c => c.GetAllMatchesAsync()).ReturnsAsync(new[] { finished, planned });

        IReadOnlyCollection<MatchDto>? passedMatches = null;
        _coordinator
            .Setup(c => c.InitializeAsync(It.IsAny<IReadOnlyCollection<MatchDto>>()))
            .Callback((IReadOnlyCollection<MatchDto> matches) => passedMatches = matches)
            .Returns(Task.CompletedTask);

        await action();

        Assert.NotNull(passedMatches);
        return ((finished, planned), passedMatches!);
    }
}
