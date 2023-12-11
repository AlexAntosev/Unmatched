namespace Unmatched.Tests;

using System;
using Moq;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services;
using Unmatched.Services.MatchHandlers;

using Match = Unmatched.Entities.Match;

public class UnrankedMatchHandlerTests
{
    private readonly Mock<IMatchRepository> _matchRepository;
    private readonly Mock<IFighterRepository> _fighterRepository;

    private readonly UnrankedMatchHandler _handler;

    public UnrankedMatchHandlerTests()
    {
        _matchRepository = new Mock<IMatchRepository>();
        _fighterRepository = new Mock<IFighterRepository>();
        _handler = new UnrankedMatchHandler(_matchRepository.Object, _fighterRepository.Object);
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
        _fighterRepository.VerifyAll();
    }
}
