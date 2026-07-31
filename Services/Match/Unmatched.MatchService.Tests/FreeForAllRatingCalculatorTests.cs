namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

public class FreeForAllRatingCalculatorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    private readonly FreeForAllRatingCalculator _calculator;

    public FreeForAllRatingCalculatorTests()
    {
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _calculator = new FreeForAllRatingCalculator(_unitOfWork.Object, _catalogHeroCache.Object);
    }

    [Fact]
    public async Task CalculateAsync_FourEqualFighters_PairwiseDecompositionSumsToExactlyZero()
    {
        var fighters = Enumerable.Range(1, 4).Select(placement => Guid.NewGuid()).ToList();
        foreach (var heroId in fighters)
        {
            SetUpHero(heroId);
            SetUpRating(heroId, 1000);
        }

        var match = CreateMatch(fighters);

        var result = await _calculator.CalculateAsync(match);

        Assert.Equal(0, fighters.Sum(id => result[id]));
    }

    [Fact]
    public async Task CalculateAsync_FourEqualFighters_EachPlaceBeatsTheNextByTheSameMargin()
    {
        // equal ratings + a maxed-out winner (full HP, full deck, no sidekick) makes the pairwise math
        // exact: every one of the 3 comparisons a fighter is in contributes ±0.5 or a symmetric win/
        // loss against another equal-rated fighter, so places should be evenly spaced.
        var fighters = Enumerable.Range(1, 4).Select(_ => Guid.NewGuid()).ToList();
        foreach (var heroId in fighters)
        {
            SetUpHero(heroId);
            SetUpRating(heroId, 1000);
        }

        var result = await _calculator.CalculateAsync(CreateMatch(fighters));

        var deltas = fighters.Select(id => result[id]).ToList();
        Assert.Equal(deltas[0] - deltas[1], deltas[1] - deltas[2]);
        Assert.Equal(deltas[1] - deltas[2], deltas[2] - deltas[3]);
        Assert.True(deltas[0] > deltas[1]);
        Assert.True(deltas[1] > deltas[2]);
        Assert.True(deltas[2] > deltas[3]);
    }

    [Fact]
    public async Task CalculateAsync_TakingFirstAsAnUnderdog_PaysMoreThanTakingFirstAsAFavourite()
    {
        var winnerId = Guid.NewGuid();
        var others = Enumerable.Range(1, 3).Select(_ => Guid.NewGuid()).ToList();
        SetUpHero(winnerId);
        foreach (var heroId in others)
        {
            SetUpHero(heroId);
        }

        SetUpRating(winnerId, 800);
        foreach (var heroId in others)
        {
            SetUpRating(heroId, 1200);
        }

        var underdogFirstResult = await _calculator.CalculateAsync(CreateMatch(new[] { winnerId }.Concat(others).ToList()));

        SetUpRating(winnerId, 1200);
        foreach (var heroId in others)
        {
            SetUpRating(heroId, 800);
        }

        var favouriteFirstResult = await _calculator.CalculateAsync(CreateMatch(new[] { winnerId }.Concat(others).ToList()));

        Assert.True(underdogFirstResult[winnerId] > favouriteFirstResult[winnerId]);
    }

    /// <summary>Fighters in placement order; the first is the winner, given full HP/cards/no sidekick
    /// so the margin-of-victory modifier is a known, deterministic value.</summary>
    private static MatchEntity CreateMatch(IReadOnlyList<Guid> heroIdsInPlacementOrder)
        => new()
        {
            Fighters = heroIdsInPlacementOrder.Select((heroId, index) => new FighterEntity
            {
                HeroId = heroId,
                Placement = index + 1,
                IsWinner = index == 0,
                HpLeft = index == 0 ? 16 : null,
                CardsLeft = index == 0 ? 10 : null,
                SidekickHpLeft = null,
            }).ToList(),
        };

    private void SetUpHero(Guid heroId)
        => _catalogHeroCache
            .Setup(c => c.GetAsync(heroId))
            .ReturnsAsync(new CatalogHeroDto { Id = heroId, Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() });

    private void SetUpRating(Guid heroId, int points)
        => _ratingRepository.Setup(r => r.GetByHeroIdAsync(heroId)).ReturnsAsync(new RatingEntity { HeroId = heroId, Points = points });
}
