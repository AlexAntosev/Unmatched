using Microsoft.Extensions.Logging;
using Moq;
using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services;
using Unmatched.Services.MatchHandlers;
using Match = Unmatched.Entities.Match;

namespace Unmatched.Tests;

using Unmatched.Services.RatingCalculators;

public class MatchHandlerFactoryTests
{
    private readonly Guid _firstTournamentId = Guid.NewGuid();
    private readonly Guid _goldenHalatLeagueId = Guid.NewGuid();

    private readonly Mock<ILoggerFactory> _loggerFactory = new();
    private readonly Mock<ITournamentRepository> _tournamentRepository = new();
    private readonly Mock<IRatingCalculator> _ratingCalculator = new();
    private readonly Mock<IFirstTournamentRatingCalculator> _firstTournamentRatingCalculator = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IFighterRepository> _fighterRepository = new();
    private readonly Mock<IMatchStageRepository> _matchStageRepository = new();

    private readonly MatchHandlerFactory _factory;

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
        
        _factory = new MatchHandlerFactory(
            _loggerFactory.Object,
            _tournamentRepository.Object,
            _ratingCalculator.Object,
            _firstTournamentRatingCalculator.Object,
            _matchRepository.Object,
            _ratingRepository.Object,
            _fighterRepository.Object,
            _matchStageRepository.Object);
    }

    [Fact]
    public void Create_PassFirstTournamentMatch_CreateFirstTournamentHandler()
    {
        var match = new Match
            {
                TournamentId = _firstTournamentId
            };
        var handler = _factory.Create(match);
        Assert.IsType<FirstTournamentMatchHandler>(handler);
    }

    [Fact]
    public void Create_PassGoldenHalatLeagueMatch_CreateGoldenHalatLeagueHandler()
    {
        var match = new Match
            {
                TournamentId = _goldenHalatLeagueId
            };
        var handler = _factory.Create(match);
        Assert.IsType<GoldenHalatLeagueMatchHandler>(handler);
    }

    [Fact]
    public void Create_PassUnrankedMatch_CreateUnrankedHandler()
    {
        var match = new Match();
        var handler = _factory.Create(match);
        Assert.IsType<UnrankedMatchHandler>(handler);
    }
}
