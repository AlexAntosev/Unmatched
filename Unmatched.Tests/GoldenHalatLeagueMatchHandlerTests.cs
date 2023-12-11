using Moq;
using Unmatched.Services;
using Unmatched.Services.MatchHandlers;
using Unmatched.Entities;
using Match = Unmatched.Entities.Match;

namespace Unmatched.Tests;

using Unmatched.Repositories;

public class GoldenHalatLeagueMatchHandlerTests
{
    private readonly Mock<IRatingCalculator> _ratingCalculator;
    private readonly Mock<IMatchRepository> _matchRepository;
    private readonly Mock<IRatingRepository> _ratingRepository;
    private readonly Mock<IFighterRepository> _fighterRepository;

    private readonly GoldenHalatLeagueMatchHandler _handler;

    public GoldenHalatLeagueMatchHandlerTests()
    {
        _ratingCalculator = new Mock<IRatingCalculator>();
        _matchRepository = new Mock<IMatchRepository>();
        _ratingRepository = new Mock<IRatingRepository>();
        _fighterRepository = new Mock<IFighterRepository>();
        _handler = new GoldenHalatLeagueMatchHandler(_ratingCalculator.Object, _matchRepository.Object, _ratingRepository.Object, _fighterRepository.Object);
    }
    
    [Fact]
    public async Task HandleAsync_CalculateRatingForMatchFighters()
    {
        // Arrange
        var fighterHeroId = Guid.NewGuid();
        var fighter = new Fighter
            {
                HeroId = fighterHeroId
            };
        var opponentHeroId = Guid.NewGuid();
        var opponent = new Fighter
            {
                HeroId = opponentHeroId
            };
        var match = new Match
            {
                Fighters = new List<Fighter>
                    {
                        fighter,
                        opponent
                    }
            };
        var matchPoints = new List<HeroMatchPoints>()
            {
                new()
                    {
                        HeroId = fighterHeroId
                    },
                new()
                    {
                        HeroId = opponentHeroId
                    }
            };
        
        _ratingCalculator
            .Setup(c => c.CalculateAsync(fighter, opponent))
            .ReturnsAsync(matchPoints)
            .Verifiable();

        var createdMatchId = Guid.NewGuid();
        var createdMatch = new Match()
            {
                Id = createdMatchId
            };
        _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch);
        
        // Act
        await _handler.HandleAsync(match);
        
        // Assert
        _ratingCalculator.VerifyAll();
    }
    
    [Fact]
    public async Task HandleAsync_CreatesMatchEntity()
    {
        // Arrange
        var fighterHeroId = Guid.NewGuid();
        var fighter = new Fighter
            {
                HeroId = fighterHeroId
            };
        var opponentHeroId = Guid.NewGuid();
        var opponent = new Fighter
            {
                HeroId = opponentHeroId
            };
        var match = new Match
            {
                Fighters = new List<Fighter>
                    {
                        fighter,
                        opponent
                    }
            };
        var matchPoints = new List<HeroMatchPoints>()
            {
                new()
                    {
                        HeroId = fighterHeroId
                    },
                new()
                    {
                        HeroId = opponentHeroId
                    }
            };
        
        _ratingCalculator
            .Setup(c => c.CalculateAsync(fighter, opponent))
            .ReturnsAsync(matchPoints);
        
        var createdMatchId = Guid.NewGuid();
        var createdMatch = new Match()
            {
                Id = createdMatchId
            };
        _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch).Verifiable();
        _matchRepository.Setup(r => r.SaveChangesAsync()).Verifiable();
        
        // Act
        await _handler.HandleAsync(match);
        
        // Assert
        _matchRepository.VerifyAll();
    }
    
    [Fact]
    public async Task HandleAsync_CreatesFightersEntities()
    {
        // Arrange
        var fighterHeroId = Guid.NewGuid();
        var fighter = new Fighter
            {
                HeroId = fighterHeroId
            };
        var opponentHeroId = Guid.NewGuid();
        var opponent = new Fighter
            {
                HeroId = opponentHeroId
            };
        var match = new Match
            {
                Fighters = new List<Fighter>
                    {
                        fighter,
                        opponent
                    }
            };
        var fighterMatchPoints = 100;
        var opponentMatchPoints = -100;
        var matchPoints = new List<HeroMatchPoints>()
            {
                new()
                    {
                        HeroId = fighterHeroId,
                        Points = fighterMatchPoints
                    },
                new()
                    {
                        HeroId = opponentHeroId,
                        Points = opponentMatchPoints
                    }
            };
        
        _ratingCalculator
            .Setup(c => c.CalculateAsync(fighter, opponent))
            .ReturnsAsync(matchPoints);
        
        var createdMatchId = Guid.NewGuid();
        var createdMatch = new Match()
            {
                Id = createdMatchId
            };
        _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch).Verifiable();
        _fighterRepository.Setup(r => r.AddAsync(fighter)).Verifiable();
        _fighterRepository.Setup(r => r.AddAsync(opponent)).Verifiable();
        _fighterRepository.Setup(r => r.SaveChangesAsync()).Verifiable();

        // Act
        await _handler.HandleAsync(match);
        
        // Assert
        Assert.Equal(createdMatchId, fighter.MatchId);
        Assert.Equal(createdMatchId, opponent.MatchId);
        Assert.Equal(fighterMatchPoints, fighter.MatchPoints);
        Assert.Equal(opponentMatchPoints, opponent.MatchPoints);
        _fighterRepository.VerifyAll();
    }
    
    [Fact]
    public async Task HandleAsync_UpdatesHeroRating()
    {
        // Arrange
        var fighterHeroId = Guid.NewGuid();
        var fighter = new Fighter
            {
                HeroId = fighterHeroId
            };
        var opponentHeroId = Guid.NewGuid();
        var opponent = new Fighter
            {
                HeroId = opponentHeroId
            };
        var match = new Match
            {
                Fighters = new List<Fighter>
                    {
                        fighter,
                        opponent
                    }
            };
        var fighterHeroRating = new Rating()
            {
                Id = fighterHeroId,
                Points = 100
            };
        var opponentHeroRating = new Rating()
            {
                Id = opponentHeroId,
                Points = 100
            };
        var fighterMatchPoints = 100;
        var opponentMatchPoints = -100;
        var matchPoints = new List<HeroMatchPoints>()
            {
                new()
                    {
                        HeroId = fighterHeroId,
                        Points = fighterMatchPoints
                    },
                new()
                    {
                        HeroId = opponentHeroId,
                        Points = opponentMatchPoints
                    }
            };
        
        _ratingCalculator
            .Setup(c => c.CalculateAsync(fighter, opponent))
            .ReturnsAsync(matchPoints);
        
        var createdMatchId = Guid.NewGuid();
        var createdMatch = new Match()
            {
                Id = createdMatchId
            };
        _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch);
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(fighterHeroId)).ReturnsAsync(fighterHeroRating).Verifiable();
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(opponentHeroId)).ReturnsAsync(opponentHeroRating).Verifiable();
        _ratingRepository.Setup(r => r.AddOrUpdate(fighterHeroRating)).Verifiable();
        _ratingRepository.Setup(r => r.AddOrUpdate(opponentHeroRating)).Verifiable();
        _ratingRepository.Setup(r => r.SaveChangesAsync()).Verifiable();
        
        // Act
        await _handler.HandleAsync(match);
        
        // Assert
        _ratingRepository.VerifyAll();
        Assert.Equal(200, fighterHeroRating.Points);
        Assert.Equal(0, opponentHeroRating.Points);
    }
}
