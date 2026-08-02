namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>
/// The real replacement for the old (fully commented-out) MatchHandlerFactoryTests: with tournament
/// dispatch gone, selection is a total function of GameMode alone.
/// </summary>
public class RatingCalculatorFactoryTests
{
    private readonly RatingCalculatorFactory _factory = new(
        new Mock<IUnitOfWork>().Object,
        new Mock<ICatalogHeroCache>().Object);

    [Fact]
    public void Create_OneVsOne_ReturnsOneVsOneCalculator()
        => Assert.IsType<OneVsOneRatingCalculator>(_factory.Create(GameMode.OneVsOne));

    [Fact]
    public void Create_TeamVsTeam_ReturnsTeamVsTeamCalculator()
        => Assert.IsType<TeamVsTeamRatingCalculator>(_factory.Create(GameMode.TeamVsTeam));

    [Fact]
    public void Create_FreeForAll_ReturnsFreeForAllCalculator()
        => Assert.IsType<FreeForAllRatingCalculator>(_factory.Create(GameMode.FreeForAll));

    [Fact]
    public void Create_Cooperative_ReturnsNoRatingCalculator()
        => Assert.IsType<NoRatingCalculator>(_factory.Create(GameMode.Cooperative));

    [Fact]
    public void Create_UnknownGameMode_Throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => _factory.Create((GameMode)999));
}
