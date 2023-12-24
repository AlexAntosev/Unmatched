namespace Unmatched.Tests;

using System;
using Moq;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.MatchHandlers;
using Unmatched.Services.RatingCalculators;
using Match = Unmatched.Entities.Match;

public class UnrankedMatchHandlerTests
{
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<IUnrankedRatingCalculator> _unrankedRatingCalculator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly UnrankedMatchHandler _handler;

    public UnrankedMatchHandlerTests()
    {
        _unitOfWork.Setup(uow => uow.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(uow => uow.Ratings).Returns(_ratingRepository.Object);

        _handler = new UnrankedMatchHandler(_unitOfWork.Object, _unrankedRatingCalculator.Object);
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
        
        var createdMatchId = Guid.NewGuid();
        var createdMatch = new Match()
            {
                Fighters = new List<Fighter>
                {
                    fighter,
                    opponent
                },
                Id = createdMatchId
            };
        _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch).Verifiable();
        _unitOfWork.Setup(r => r.SaveChangesAsync()).Verifiable();
        
        _unrankedRatingCalculator.Setup(c => c.CalculateAsync(fighter, opponent))
            .ReturnsAsync(
                new Dictionary<Guid, int>()
                    {
                        {
                            fighterHeroId, 0
                        },
                        {
                            opponentHeroId, 0
                        }
                    });
        
        // Act
        await _handler.HandleAsync(match);
        
        // Assert
        _matchRepository.VerifyAll();
        _unitOfWork.VerifyAll();
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
        
        var createdMatchId = Guid.NewGuid();
        var createdMatch = new Match()
            {
                Fighters = new List<Fighter>
                {
                    fighter,
                    opponent
                },
                Id = createdMatchId
            };
        _matchRepository.Setup(r => r.AddAsync(match)).Callback((Match match) =>
            {
                foreach (var fighter in match.Fighters)
                {
                    fighter.MatchId = createdMatch.Id;
                }
            }).ReturnsAsync(createdMatch).Verifiable();
        _unrankedRatingCalculator.Setup(c => c.CalculateAsync(fighter, opponent))
            .ReturnsAsync(
                new Dictionary<Guid, int>()
                    {
                        {
                            fighterHeroId, 0
                        },
                        {
                            opponentHeroId, 0
                        }
                    });

        // Act
        await _handler.HandleAsync(match);
        
        // Assert
        Assert.Equal(createdMatchId, fighter.MatchId);
        Assert.Equal(createdMatchId, opponent.MatchId);
    }
}
