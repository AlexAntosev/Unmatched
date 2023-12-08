namespace Unmatched.Tests;

using Microsoft.Extensions.Logging;

using Moq;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.MatchHandlers;

using Match = Unmatched.Entities.Match;

public class MatchHandlerFactoryTests
{
    private readonly Guid _firstTournamentId = Guid.NewGuid();

    private readonly Guid _goldenHalatLeagueId = Guid.NewGuid();

    private readonly Mock<ILoggerFactory> _loggerFactory = new();

    private readonly Mock<ITournamentRepository> _tournamentRepository = new();

    public MatchHandlerFactoryTests()
    {
        _tournamentRepository.Setup(x => x.Query())
            .Returns(
                new List<Tournament>()
                    {
                        new()
                            {
                                Name = TournamentNames.UnmatchedFirstTournament,
                                Id = _firstTournamentId
                            },
                        new()
                            {
                                Name = TournamentNames.GoldenHalatLeague,
                                Id = _goldenHalatLeagueId
                            }
                    }.AsQueryable());
    }

    [Fact]
    public void Create_PassFirstTournamentMatch_CreateFirstTournamentHandler()
    {
        var match = new Match()
            {
                TournamentId = _firstTournamentId
            };
        var factory = new MatchHandlerFactory(_loggerFactory.Object, _tournamentRepository.Object);
        var handler = factory.Create(match);
        Assert.IsType<FirstTournamentMatchHandler>(handler);
    }

    [Fact]
    public void Create_PassGoldenHalatLeagueMatch_CreateGoldenHalatLeagueHandler()
    {
        var match = new Match()
            {
                TournamentId = _goldenHalatLeagueId
            };
        var factory = new MatchHandlerFactory(_loggerFactory.Object, _tournamentRepository.Object);
        var handler = factory.Create(match);
        Assert.IsType<GoldenHalatLeagueMatchHandler>(handler);
    }

    [Fact]
    public void Create_PassUnrankedMatch_CreateUnrankedHandler()
    {
        var match = new Match();
        var factory = new MatchHandlerFactory(_loggerFactory.Object, _tournamentRepository.Object);
        var handler = factory.Create(match);
        Assert.IsType<UnrankedMatchHandler>(handler);
    }
}
