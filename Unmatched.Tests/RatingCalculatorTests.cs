using Moq;
using Unmatched.Constants;
using Unmatched.DataInitialization;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services;

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
                new Fighter
                {
                    HeroId = drSatId, HpLeft = 8, CardsLeft = 10, SidekickHpLeft = 1, Turn = 2, IsWinner = true
                },
                new Fighter
                {
                    HeroId = daredevilId, HpLeft = 0, CardsLeft = 5, SidekickHpLeft = 0, Turn = 1, IsWinner = false
                }),
            new HeroMatchPoints { HeroId = drSatId, Points = 420 },
            new HeroMatchPoints { HeroId = daredevilId, Points = -225 });
        
        AssertMatch(await calculator.CalculateAsync(
                new Fighter
                {
                    HeroId = robinHoodId, HpLeft = 8, CardsLeft = 13, SidekickHpLeft = 1, Turn = 2, IsWinner = true
                },
                new Fighter
                {
                    HeroId = ingenId, HpLeft = 0, CardsLeft = 16, SidekickHpLeft = 0, Turn = 1, IsWinner = false
                }),
            new HeroMatchPoints { HeroId = robinHoodId, Points = 336 },
            new HeroMatchPoints { HeroId = ingenId, Points = 0 });
        
        AssertMatch(await calculator.CalculateAsync(
                new Fighter
                {
                    HeroId = rexId, HpLeft = 17, CardsLeft = 16, SidekickHpLeft = 0, Turn = 1, IsWinner = true
                },
                new Fighter
                {
                    HeroId = sherlokHolmesId, HpLeft = 0, CardsLeft = 18, SidekickHpLeft = 7, Turn = 2, IsWinner = false
                }),
            new HeroMatchPoints { HeroId = rexId, Points = 316 },
            new HeroMatchPoints { HeroId = sherlokHolmesId, Points = -100 });
        
        AssertMatch(await calculator.CalculateAsync(
                new Fighter
                {
                    HeroId = invisibleManId, HpLeft = 6, CardsLeft = 7, SidekickHpLeft = 0, Turn = 2, IsWinner = true
                },
                new Fighter
                {
                    HeroId = sindbadId, HpLeft = 0, CardsLeft = 9, SidekickHpLeft = 5, Turn = 1, IsWinner = false
                }),
            new HeroMatchPoints { HeroId = invisibleManId, Points = 360 },
            new HeroMatchPoints { HeroId = sindbadId, Points = -165 });
        
        AssertMatch(await calculator.CalculateAsync(
                new Fighter
                {
                    HeroId = raptorsId, HpLeft = 8, CardsLeft = 11, SidekickHpLeft = 0, Turn = 1, IsWinner = true
                },
                new Fighter
                {
                    HeroId = draculaId, HpLeft = 0, CardsLeft = 10, SidekickHpLeft = 1, Turn = 2, IsWinner = false
                }),
            new HeroMatchPoints { HeroId = raptorsId, Points = 496 },
            new HeroMatchPoints { HeroId = draculaId, Points = -190 });
        
        AssertMatch(await calculator.CalculateAsync(
                new Fighter
                {
                    HeroId = medusaId, HpLeft = 5, CardsLeft = 2, SidekickHpLeft = 0, Turn = 1, IsWinner = true
                },
                new Fighter
                {
                    HeroId = bloodyMaryId, HpLeft = 0, CardsLeft = 1, SidekickHpLeft =0, Turn = 2, IsWinner = false
                }),
            new HeroMatchPoints { HeroId = medusaId, Points = 272 },
            new HeroMatchPoints { HeroId = bloodyMaryId, Points = -195 });
    }

    private void AssertMatch(IEnumerable<HeroMatchPoints> matchPoints, HeroMatchPoints winnerExpectedPoints,
        HeroMatchPoints looserExpectedPoints)
    {
        var actualWinnerPoints = matchPoints.First(x => x.HeroId.Equals(winnerExpectedPoints.HeroId)).Points;
        var actualLooserPoints = matchPoints.First(x => x.HeroId.Equals(looserExpectedPoints.HeroId)).Points;

        Assert.Equal(winnerExpectedPoints.Points, actualWinnerPoints);
        Assert.Equal(looserExpectedPoints.Points, actualLooserPoints);
    }

    private IRatingRepository CreateMockRatingRepository(IEnumerable<Hero> heroes)
    {
        var mockRepo = new Mock<IRatingRepository>();

        var ratings = new List<Rating>
        {
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.DrSattler)).Id, Points = 0 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.Daredevil)).Id, Points = 700 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.RobinHood)).Id, Points = 1500 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.Ingen)).Id, Points = 0 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.TRex)).Id, Points = 0 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.SherlokHolmes)).Id, Points = 100 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.InvisibleMan)).Id, Points = 400 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.Sindbad)).Id, Points = 1200 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.Raptors)).Id, Points = 0 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.Dracula)).Id, Points = 1000 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.Medusa)).Id, Points = 1300 },
            new() { HeroId = heroes.First(x => x.Name.Equals(HeroNames.BloodyMary)).Id, Points = 1700 }
        };
        mockRepo.Setup(x => x.GetByHeroIdAsync(It.IsAny<Guid>()))
            .Returns((Guid heroId) => Task.FromResult(ratings.First(x => x.HeroId == heroId)));

        return mockRepo.Object;
    }

    private IHeroRepository CreateMockHeroRepository()
    {
        var mockSidekickRepository = new Mock<ISidekickRepository>();
        mockSidekickRepository.Setup(x => x.Query()).Returns(new SidekicksDataInitializer(null).GetEntities());

        var heroDataInitializer = new HeroesDataInitializer(null, mockSidekickRepository.Object);

        var mockHeroRepository = new Mock<IHeroRepository>();
        var heroes = heroDataInitializer.GetEntities();
        mockHeroRepository.Setup(x => x.Query()).Returns(heroes);
        mockHeroRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .Returns((Guid id) => Task.FromResult(heroes.First(x => x.Id == id)));

        return mockHeroRepository.Object;
    }
}