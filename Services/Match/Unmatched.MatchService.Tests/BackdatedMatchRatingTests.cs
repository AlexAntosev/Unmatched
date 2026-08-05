namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.MatchHandlers;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

/// <summary>
/// Rating math is stateful and sequential: each match's handicap bonus depends on the hero ratings
/// accumulated from every match processed *before* it. These tests demonstrate that processing matches
/// out of chronological order (as happens when a match is added with a backdated Date after later matches
/// already exist) produces a different result than the true chronological order - and that replaying
/// matches sorted by Date (what RecalculateAsync now does) recovers the correct result regardless of the
/// order they were originally inserted in.
/// </summary>
public class BackdatedMatchRatingTests
{
    private static readonly Guid HeroAId = Guid.NewGuid();
    private static readonly Guid HeroBId = Guid.NewGuid();

    [Fact]
    public async Task ProcessingMatchesOutOfChronologicalOrder_ProducesDifferentRatingsThanProcessingInOrder()
    {
        // January: Hero A beats Hero B. February: Hero B beats Hero A.
        var matchJan = CreateMatch(winnerHeroId: HeroAId, looserHeroId: HeroBId);
        var matchFeb = CreateMatch(winnerHeroId: HeroBId, looserHeroId: HeroAId);

        // correct chronological order: Jan then Feb
        var (chronologicalA, chronologicalB) = await PlayMatchesInOrderAsync(matchJan, matchFeb);

        // backdated insertion: Feb gets processed first (e.g. already in the DB), Jan is added afterwards
        // with an earlier Date - simulating a match added "заднім числом"
        var (backdatedA, backdatedB) = await PlayMatchesInOrderAsync(matchFeb, matchJan);

        Assert.NotEqual((chronologicalA, chronologicalB), (backdatedA, backdatedB));
    }

    [Fact]
    public async Task ReplayingMatchesSortedByDate_RecoversTheCorrectChronologicalRatings_RegardlessOfInsertionOrder()
    {
        var matchJan = CreateMatch(winnerHeroId: HeroAId, looserHeroId: HeroBId, date: new DateTime(2026, 1, 1));
        var matchFeb = CreateMatch(winnerHeroId: HeroBId, looserHeroId: HeroAId, date: new DateTime(2026, 2, 1));

        var (expectedA, expectedB) = await PlayMatchesInOrderAsync(matchJan, matchFeb);

        // matchJan is the backdated one: it was inserted/stored *after* matchFeb.
        // A correct recalculation must still replay in Date order (Jan, then Feb), not insertion order.
        var matchesInStorageOrder = new[] { matchFeb, matchJan };
        var (recalculatedA, recalculatedB) = await PlayMatchesInOrderAsync(matchesInStorageOrder.OrderBy(m => m.Date).ToArray());

        Assert.Equal(expectedA, recalculatedA);
        Assert.Equal(expectedB, recalculatedB);
    }

    private static MatchEntity CreateMatch(Guid winnerHeroId, Guid looserHeroId, DateTime? date = null)
    {
        return new MatchEntity
        {
            IsRanked = true,
            GameMode = GameMode.OneVsOne,
            Date = date ?? DateTime.UtcNow,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = winnerHeroId, IsWinner = true, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 },
                new() { HeroId = looserHeroId, IsWinner = false, HpLeft = 0, SidekickHpLeft = 0, CardsLeft = 0 },
            },
        };
    }

    private static async Task<(int HeroAPoints, int HeroBPoints)> PlayMatchesInOrderAsync(params MatchEntity[] matches)
    {
        var catalogHeroCache = new Mock<ICatalogHeroCache>();
        catalogHeroCache.Setup(c => c.GetAsync(HeroAId)).ReturnsAsync(new CatalogHeroDto { Id = HeroAId, Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() });
        catalogHeroCache.Setup(c => c.GetAsync(HeroBId)).ReturnsAsync(new CatalogHeroDto { Id = HeroBId, Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() });

        var ratingRepository = new FakeRatingRepository();

        var matchRepository = new Mock<IMatchRepository>();
        matchRepository.Setup(r => r.AddAsync(It.IsAny<MatchEntity>())).ReturnsAsync((MatchEntity m) =>
        {
            m.Id = Guid.NewGuid();
            return m;
        });

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Matches).Returns(matchRepository.Object);
        unitOfWork.Setup(u => u.Ratings).Returns(ratingRepository);

        var handler = new MatchHandler(
            unitOfWork.Object,
            new GameModeValidatorFactory(),
            new RatingCalculatorFactory(unitOfWork.Object, catalogHeroCache.Object),
            new BountyRatingCalculator(unitOfWork.Object, catalogHeroCache.Object),
            new Domain.Tournaments.BountyHolderTitleUpdater(unitOfWork.Object));

        foreach (var match in matches)
        {
            await handler.HandleAsync(match);
        }

        var heroAPoints = (await ratingRepository.GetByHeroIdAsync(HeroAId))?.Points ?? 0;
        var heroBPoints = (await ratingRepository.GetByHeroIdAsync(HeroBId))?.Points ?? 0;
        return (heroAPoints, heroBPoints);
    }

    private class FakeRatingRepository : IRatingRepository
    {
        private readonly Dictionary<Guid, RatingEntity> _byHeroId = new();

        public Task<RatingEntity?> GetByHeroIdAsync(Guid heroId)
            => Task.FromResult(_byHeroId.TryGetValue(heroId, out var rating) ? rating : null);

        public Task<RatingEntity> AddAsync(RatingEntity model)
        {
            _byHeroId[model.HeroId] = model;
            return Task.FromResult(model);
        }

        public void AddOrUpdate(RatingEntity model, Guid id) => _byHeroId[model.HeroId] = model;

        public void AddOrUpdate(RatingEntity model) => _byHeroId[model.HeroId] = model;

        public Task AddOrUpdateAsync(RatingEntity model)
        {
            AddOrUpdate(model);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<RatingEntity> models)
        {
            foreach (var model in models)
            {
                AddOrUpdate(model);
            }

            return Task.CompletedTask;
        }

        public Task Delete(Guid id)
        {
            var entry = _byHeroId.Values.FirstOrDefault(r => r.Id == id);
            if (entry != null)
            {
                _byHeroId.Remove(entry.HeroId);
            }

            return Task.CompletedTask;
        }

        public void DeleteAll() => _byHeroId.Clear();

        public IReadOnlyList<RatingEntity> Get() => _byHeroId.Values.ToList();

        public Task<IReadOnlyList<RatingEntity>> GetAsync() => Task.FromResult((IReadOnlyList<RatingEntity>)_byHeroId.Values.ToList());

        public Task<RatingEntity?> GetByIdAsync(Guid id) => Task.FromResult(_byHeroId.Values.FirstOrDefault(r => r.Id == id));

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
