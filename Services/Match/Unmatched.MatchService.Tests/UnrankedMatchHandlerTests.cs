namespace Unmatched.MatchService.Tests;

using System;

using Moq;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

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
        var fighter = new FighterEntity
        {
                HeroId = fighterHeroId
            };
        var opponentHeroId = Guid.NewGuid();
        var opponent = new FighterEntity
        {
                HeroId = opponentHeroId
            };
        var match = new MatchEntity
            {
                Fighters = new List<FighterEntity>
                    {
                        fighter,
                        opponent
                    }
            };
        
        var createdMatchId = Guid.NewGuid();
        var createdMatch = new MatchEntity()
            {
                Fighters = new List<FighterEntity>
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
        var fighter = new FighterEntity()
            {
                HeroId = fighterHeroId
            };
        var opponentHeroId = Guid.NewGuid();
        var opponent = new FighterEntity
        {
                HeroId = opponentHeroId
            };
        var match = new MatchEntity()
            {
                Fighters = new List<FighterEntity>
                    {
                        fighter,
                        opponent
                    }
            };
        
        var createdMatchId = Guid.NewGuid();
        var createdMatch = new MatchEntity()
            {
                Fighters = new List<FighterEntity>
                {
                    fighter,
                    opponent
                },
                Id = createdMatchId
            };
        _matchRepository.Setup(r => r.AddAsync(match)).Callback((MatchEntity match) =>
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
