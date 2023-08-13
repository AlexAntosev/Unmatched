using Moq;
using Unmatched.Constants;
using Unmatched.DataInitialization;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services;
using Xunit.Sdk;

namespace Unmatched.Tests;

public class RatingCalculatorTests
{
    [Fact]
    public async void CheckCalculations()
    {
        var heroRepository = CreateMockHeroRepository();
        var heroes = heroRepository.Query().ToList();
        
        var ratingRepository = CreateMockRatingRepository(heroes);
        
        var calculator = new RatingCalculator(ratingRepository, heroRepository);
        
        var drSatId = heroes.First(x => x.Name.Equals(HeroNames.DrSattler)).Id;
        var daredevilId = heroes.First(x => x.Name.Equals(HeroNames.Daredevil)).Id;
        var robinHoodId = heroes.First(x => x.Name.Equals(HeroNames.RobinHood)).Id;
        var ingenId = heroes.First(x => x.Name.Equals(HeroNames.Ingen)).Id;
        var rexId = heroes.First(x => x.Name.Equals(HeroNames.TRex)).Id;
        var sherlokHolmesId = heroes.First(x => x.Name.Equals(HeroNames.SherlokHolmes)).Id;
        var invisibleManId = heroes.First(x => x.Name.Equals(HeroNames.InvisibleMan)).Id;
        var sindbadId = heroes.First(x => x.Name.Equals(HeroNames.Sindbad)).Id;
        var raptorsId = heroes.First(x => x.Name.Equals(HeroNames.Raptors)).Id;
        var draculaId = heroes.First(x => x.Name.Equals(HeroNames.Dracula)).Id;
        var medusaId = heroes.First(x => x.Name.Equals(HeroNames.Medusa)).Id;
        var bloodyMaryId = heroes.First(x => x.Name.Equals(HeroNames.BloodyMary)).Id;

        AssertMatch(await calculator.CalculateAsync(
                new Fighter()
                    { HeroId = drSatId, HpLeft = 8, CardsLeft = 10, SidekickHpLeft = 1, Turn = 2, IsWinner = true },
                new Fighter()
                    { HeroId = daredevilId, HpLeft = 0, CardsLeft = 5, SidekickHpLeft = 0, Turn = 1, IsWinner = false }),
            new HeroMatchPoints() { HeroId = drSatId, Points = 420 },
            new HeroMatchPoints() { HeroId = daredevilId, Points = -225 });
    }

    private void AssertMatch(IEnumerable<HeroMatchPoints> matchPoints, HeroMatchPoints winnerExpectedPoints, HeroMatchPoints looserExpectedPoints)
    {
        var actualWinnerPoints = matchPoints.First(x => x.HeroId.Equals(winnerExpectedPoints.HeroId)).Points;
        var actualLooserPoints = matchPoints.First(x => x.HeroId.Equals(looserExpectedPoints.HeroId)).Points;
        
        Assert.Equal(winnerExpectedPoints.Points, actualWinnerPoints);
        Assert.Equal(looserExpectedPoints.Points, actualLooserPoints);
    }

    private IRatingRepository CreateMockRatingRepository(IEnumerable<Hero> heroes)
    {
        var mockRepo = new Mock<IRatingRepository>();
        
        var drSatId = heroes.First(x => x.Name.Equals(HeroNames.DrSattler)).Id;
        var daredevilId = heroes.First(x => x.Name.Equals(HeroNames.Daredevil)).Id;
        var robinHoodId = heroes.First(x => x.Name.Equals(HeroNames.RobinHood)).Id;
        var ingenId = heroes.First(x => x.Name.Equals(HeroNames.Ingen)).Id;
        var rexId = heroes.First(x => x.Name.Equals(HeroNames.TRex)).Id;
        var sherlokHolmesId = heroes.First(x => x.Name.Equals(HeroNames.SherlokHolmes)).Id;
        var invisibleManId = heroes.First(x => x.Name.Equals(HeroNames.InvisibleMan)).Id;
        var sindbadId = heroes.First(x => x.Name.Equals(HeroNames.Sindbad)).Id;
        var raptorsId = heroes.First(x => x.Name.Equals(HeroNames.Raptors)).Id;
        var draculaId = heroes.First(x => x.Name.Equals(HeroNames.Dracula)).Id;
        var medusaId = heroes.First(x => x.Name.Equals(HeroNames.Medusa)).Id;
        var bloodyMaryId = heroes.First(x => x.Name.Equals(HeroNames.BloodyMary)).Id;

        var ratings = new List<Rating>()
        {
            new Rating() { HeroId = drSatId, Points = 0 },
            new Rating() { HeroId = daredevilId, Points = 700 },
            new Rating(){HeroId = robinHoodId,Points = 1500}
        };
        mockRepo.Setup(x => x.GetByHeroIdAsync(It.IsAny<Guid>())).Returns((Guid heroId) => Task.FromResult(ratings.First(x => x.HeroId == heroId)));

        // mockRepo.Setup(x => x.GetByIdAsync(drSatId)).Returns(Task.FromResult(new Rating(){HeroId = drSatId,Points = 0}));
        // mockRepo.Setup(x => x.GetByIdAsync(daredevilId)).Returns(Task.FromResult(new Rating(){HeroId = daredevilId,Points = 700}));
        // mockRepo.Setup(x => x.GetByIdAsync(robinHoodId)).Returns(Task.FromResult(new Rating(){HeroId = robinHoodId,Points = 1500}));
        // mockRepo.Setup(x => x.GetByIdAsync(ingenId)).Returns(Task.FromResult(new Rating(){HeroId = ingenId,Points = 0}));
        // mockRepo.Setup(x => x.GetByIdAsync(rexId)).Returns(Task.FromResult(new Rating(){HeroId = rexId,Points = 0}));
        // mockRepo.Setup(x => x.GetByIdAsync(sherlokHolmesId)).Returns(Task.FromResult(new Rating(){HeroId = sherlokHolmesId,Points = 100}));
        // mockRepo.Setup(x => x.GetByIdAsync(invisibleManId)).Returns(Task.FromResult(new Rating(){HeroId = invisibleManId,Points = 400}));
        // mockRepo.Setup(x => x.GetByIdAsync(sindbadId)).Returns(Task.FromResult(new Rating(){HeroId = sindbadId,Points = 1200}));
        // mockRepo.Setup(x => x.GetByIdAsync(raptorsId)).Returns(Task.FromResult(new Rating(){HeroId = raptorsId,Points = 0}));
        // mockRepo.Setup(x => x.GetByIdAsync(draculaId)).Returns(Task.FromResult(new Rating(){HeroId = draculaId,Points = 1000}));
        // mockRepo.Setup(x => x.GetByIdAsync(medusaId)).Returns(Task.FromResult(new Rating(){HeroId = medusaId,Points = 1300}));
        // mockRepo.Setup(x => x.GetByIdAsync(bloodyMaryId)).Returns(Task.FromResult(new Rating(){HeroId = bloodyMaryId,Points = 1700}));
        return mockRepo.Object;
    }

    private IHeroRepository CreateMockHeroRepository()
    {
        var mockSidekickRepository = new Mock<ISidekickRepository>();
        mockSidekickRepository.Setup(x => x.Query()).Returns((new SidekicksDataInitializer(null)).GetEntities());

        var heroDataInitializer = new HeroesDataInitializer(null, mockSidekickRepository.Object);

        var mockHeroRepository = new Mock<IHeroRepository>();
        var heroes = heroDataInitializer.GetEntities();
        mockHeroRepository.Setup(x => x.Query()).Returns(heroes);
        mockHeroRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns((Guid id) => Task.FromResult(heroes.First(x => x.Id ==id))); 

        return mockHeroRepository.Object;
    }
}