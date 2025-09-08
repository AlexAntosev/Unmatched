// namespace Unmatched.Tests;
//
// using Moq;
//
// using Unmatched.Data.Entities;
// using Unmatched.Data.Repositories;
// using Unmatched.Services.MatchHandlers;
// using Match = Unmatched.Data.Entities.Match;
// using Unmatched.Services.RatingCalculators;
//
// public class GoldenHalatLeagueMatchHandlerTests
// {
//     private readonly Mock<IRatingCalculator> _ratingCalculator = new();
//     private readonly Mock<IMatchRepository> _matchRepository = new();
//     private readonly Mock<IRatingRepository> _ratingRepository = new();
//     private readonly Mock<IUnitOfWork> _unitOfWork = new();
//
//     private readonly GoldenHalatLeagueMatchHandler _handler;
//
//     public GoldenHalatLeagueMatchHandlerTests()
//     {
//         _unitOfWork.Setup(uow => uow.Matches).Returns(_matchRepository.Object);
//         _unitOfWork.Setup(uow => uow.Ratings).Returns(_ratingRepository.Object);
//         
//         _handler = new GoldenHalatLeagueMatchHandler(_unitOfWork.Object, _ratingCalculator.Object);
//     }
//     
//     [Fact]
//     public async Task HandleAsync_CalculateRatingForMatchFighters()
//     {
//         // Arrange
//         var fighterHeroId = Guid.NewGuid();
//         var fighter = new Fighter
//             {
//                 HeroId = fighterHeroId
//             };
//         var opponentHeroId = Guid.NewGuid();
//         var opponent = new Fighter
//             {
//                 HeroId = opponentHeroId
//             };
//         var match = new Match
//             {
//                 Fighters = new List<Fighter>
//                     {
//                         fighter,
//                         opponent
//                     }
//             };
//         var matchPoints = new Dictionary<Guid, int>
//             {
//                 {fighterHeroId, 0},
//                 {opponentHeroId, 0}
//             };
//         
//         _ratingCalculator
//             .Setup(c => c.CalculateAsync(fighter, opponent))
//             .ReturnsAsync(matchPoints)
//             .Verifiable();
//
//         var createdMatchId = Guid.NewGuid();
//         var createdMatch = new Match()
//             {
//                 Fighters = match.Fighters,
//                 Id = createdMatchId
//             };
//         _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch);
//         
//         // Act
//         await _handler.HandleAsync(match);
//         
//         // Assert
//         _ratingCalculator.VerifyAll();
//     }
//     
//     [Fact]
//     public async Task HandleAsync_CreatesMatchEntity()
//     {
//         // Arrange
//         var fighterHeroId = Guid.NewGuid();
//         var fighter = new Fighter
//             {
//                 HeroId = fighterHeroId
//             };
//         var opponentHeroId = Guid.NewGuid();
//         var opponent = new Fighter
//             {
//                 HeroId = opponentHeroId
//             };
//         var match = new Match
//             {
//                 Fighters = new List<Fighter>
//                     {
//                         fighter,
//                         opponent
//                     }
//             };
//         var matchPoints = new Dictionary<Guid, int>
//             {
//                 {fighterHeroId, 0},
//                 {opponentHeroId, 0}
//             };
//         
//         _ratingCalculator
//             .Setup(c => c.CalculateAsync(fighter, opponent))
//             .ReturnsAsync(matchPoints);
//         
//         var createdMatchId = Guid.NewGuid();
//         var createdMatch = new Match()
//             {
//                 Fighters = match.Fighters,
//                 Id = createdMatchId
//             };
//         _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch).Verifiable();
//         _unitOfWork.Setup(r => r.SaveChangesAsync()).Verifiable();
//         
//         // Act
//         await _handler.HandleAsync(match);
//         
//         // Assert
//         _matchRepository.VerifyAll();
//         _unitOfWork.VerifyAll();
//     }
//     
//     [Fact]
//     public async Task HandleAsync_CreatesFightersEntities()
//     {
//         // Arrange
//         var fighterHeroId = Guid.NewGuid();
//         var fighter = new Fighter
//             {
//                 HeroId = fighterHeroId
//             };
//         var opponentHeroId = Guid.NewGuid();
//         var opponent = new Fighter
//             {
//                 HeroId = opponentHeroId
//             };
//         var match = new Match
//             {
//                 Fighters = new List<Fighter>
//                     {
//                         fighter,
//                         opponent
//                     }
//             };
//         var fighterMatchPoints = 100;
//         var opponentMatchPoints = -100;
//         var matchPoints = new Dictionary<Guid, int>
//             {
//                 {fighterHeroId, fighterMatchPoints},
//                 {opponentHeroId, opponentMatchPoints}
//             };
//         
//         _ratingCalculator
//             .Setup(c => c.CalculateAsync(fighter, opponent))
//             .ReturnsAsync(matchPoints);
//         
//         var createdMatchId = Guid.NewGuid();
//         var createdMatch = new Match()
//             {
//                 Fighters = match.Fighters,
//                 Id = createdMatchId
//             };
//         _matchRepository.Setup(r => r.AddAsync(match)).Callback((Match match) =>
//             {
//                 foreach (var fighter in match.Fighters)
//                 {
//                     fighter.MatchId = createdMatch.Id;
//                 }
//             }).ReturnsAsync(createdMatch).Verifiable();
//         _unitOfWork.Setup(r => r.SaveChangesAsync()).Verifiable();
//
//         // Act
//         await _handler.HandleAsync(match);
//         
//         // Assert
//         Assert.Equal(createdMatchId, fighter.MatchId);
//         Assert.Equal(createdMatchId, opponent.MatchId);
//         Assert.Equal(fighterMatchPoints, fighter.MatchPoints);
//         Assert.Equal(opponentMatchPoints, opponent.MatchPoints);
//         _unitOfWork.VerifyAll();
//     }
//     
//     [Fact]
//     public async Task HandleAsync_UpdatesHeroRating()
//     {
//         // Arrange
//         var fighterHeroId = Guid.NewGuid();
//         var fighter = new Fighter
//             {
//                 HeroId = fighterHeroId
//             };
//         var opponentHeroId = Guid.NewGuid();
//         var opponent = new Fighter
//             {
//                 HeroId = opponentHeroId
//             };
//         var match = new Match
//             {
//                 Fighters = new List<Fighter>
//                     {
//                         fighter,
//                         opponent
//                     }
//             };
//         var fighterHeroRating = new Rating()
//             {
//                 Id = fighterHeroId,
//                 Points = 100
//             };
//         var opponentHeroRating = new Rating()
//             {
//                 Id = opponentHeroId,
//                 Points = 100
//             };
//         var fighterMatchPoints = 100;
//         var opponentMatchPoints = -100;
//         var matchPoints = new Dictionary<Guid, int>
//             {
//                 {fighterHeroId, fighterMatchPoints},
//                 {opponentHeroId, opponentMatchPoints}
//             };
//         
//         _ratingCalculator
//             .Setup(c => c.CalculateAsync(fighter, opponent))
//             .ReturnsAsync(matchPoints);
//         
//         var createdMatchId = Guid.NewGuid();
//         var createdMatch = new Match()
//             {
//                 Fighters = match.Fighters,
//                 Id = createdMatchId
//             };
//         _matchRepository.Setup(r => r.AddAsync(match)).ReturnsAsync(createdMatch);
//         _ratingRepository.Setup(r => r.GetByHeroIdAsync(fighterHeroId)).ReturnsAsync(fighterHeroRating).Verifiable();
//         _ratingRepository.Setup(r => r.GetByHeroIdAsync(opponentHeroId)).ReturnsAsync(opponentHeroRating).Verifiable();
//         _ratingRepository.Setup(r => r.AddOrUpdate(fighterHeroRating)).Verifiable();
//         _ratingRepository.Setup(r => r.AddOrUpdate(opponentHeroRating)).Verifiable();
//         _unitOfWork.Setup(r => r.SaveChangesAsync()).Verifiable();
//         
//         // Act
//         await _handler.HandleAsync(match);
//         
//         // Assert
//         _ratingRepository.VerifyAll();
//         _unitOfWork.VerifyAll();
//         Assert.Equal(200, fighterHeroRating.Points);
//         Assert.Equal(0, opponentHeroRating.Points);
//     }
// }
