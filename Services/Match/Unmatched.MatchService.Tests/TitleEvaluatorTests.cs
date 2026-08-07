namespace Unmatched.MatchService.Tests;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Titles;
using Unmatched.MatchService.EntityFramework.Context;
using Unmatched.MatchService.EntityFramework.Repositories;

/// <summary>Runs TitleEvaluator against a real EF Core context (in-memory provider), following the same
/// pattern as TitleServiceTests - HeroTitles add/remove reconciliation is exactly the kind of thing that
/// looks fine against mocks and silently breaks against a real change tracker.</summary>
public class TitleEvaluatorTests : IDisposable
{
    private readonly UnmatchedDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;

    public TitleEvaluatorTests()
    {
        var options = new DbContextOptionsBuilder<UnmatchedDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new UnmatchedDbContext(options);
        _unitOfWork = new UnitOfWork(_dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task EvaluateAsync_SharedRule_AddsQualifiersWithoutRemovingExistingHolders()
    {
        var existingHolder = Guid.NewGuid();
        var newQualifier = Guid.NewGuid();
        var titleId = await SeedTitleAsync("shared-rule", TitleExclusivity.Shared, existingHolder);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, newQualifier));

        await evaluator.EvaluateAsync(CreateMatch());

        var holders = await HolderIdsAsync(titleId);
        Assert.Equal(new[] { existingHolder, newQualifier }.OrderBy(x => x), holders.OrderBy(x => x));
    }

    [Fact]
    public async Task EvaluateAsync_UniqueRule_TransfersAwayFromThePreviousHolder()
    {
        var previousHolder = Guid.NewGuid();
        var newHolder = Guid.NewGuid();
        var titleId = await SeedTitleAsync("unique-rule", TitleExclusivity.Unique, previousHolder);
        var evaluator = MakeEvaluator(new StubRule("unique-rule", TitleExclusivity.Unique, newHolder));

        await evaluator.EvaluateAsync(CreateMatch());

        Assert.Equal(new[] { newHolder }, await HolderIdsAsync(titleId));
    }

    [Fact]
    public async Task EvaluateAsync_SharedRule_HeroAlreadyHolding_IncrementsTimesEarnedButReportsNothingNew()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("shared-rule", TitleExclusivity.Shared, heroId);
        var matchDate = new DateTime(2026, 6, 15);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, heroId));

        var earned = await evaluator.EvaluateAsync(CreateMatch(matchDate));

        Assert.Empty(earned);
        Assert.Equal(new[] { heroId }, await HolderIdsAsync(titleId));
        var heroTitle = await HeroTitleAsync(titleId, heroId);
        Assert.Equal(2, heroTitle.TimesEarned);
        Assert.Equal(matchDate, heroTitle.EarnedAt);
    }

    [Fact]
    public async Task EvaluateAsync_UniqueRule_HeroAlreadyHolding_LeavesTimesEarnedUntouched()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("unique-rule", TitleExclusivity.Unique, heroId);
        var evaluator = MakeEvaluator(new StubRule("unique-rule", TitleExclusivity.Unique, heroId));

        await evaluator.EvaluateAsync(CreateMatch());

        var heroTitle = await HeroTitleAsync(titleId, heroId);
        Assert.Equal(1, heroTitle.TimesEarned);
    }

    [Fact]
    public async Task EvaluateAsync_SharedRule_EarnedAcrossThreeMatches_EndsAtTimesEarnedThree()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("shared-rule", TitleExclusivity.Shared);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, heroId));

        await evaluator.EvaluateAsync(CreateMatch(new DateTime(2026, 1, 1)));
        await evaluator.EvaluateAsync(CreateMatch(new DateTime(2026, 2, 1)));
        await evaluator.EvaluateAsync(CreateMatch(new DateTime(2026, 3, 1)));

        var heroTitle = await HeroTitleAsync(titleId, heroId);
        Assert.Equal(3, heroTitle.TimesEarned);
        Assert.Equal(new DateTime(2026, 3, 1), heroTitle.EarnedAt);
    }

    [Fact]
    public async Task EvaluateAsync_NewQualifier_StampsEarnedAtWithTheMatchDate()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("shared-rule", TitleExclusivity.Shared);
        var matchDate = new DateTime(2026, 5, 1);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, heroId));

        await evaluator.EvaluateAsync(CreateMatch(matchDate));

        var earnedAt = await _dbContext.HeroTitles
            .Where(ht => ht.TitlesId == titleId && ht.HeroesId == heroId)
            .Select(ht => ht.EarnedAt)
            .SingleAsync();
        Assert.Equal(matchDate, earnedAt);
    }

    [Fact]
    public async Task EvaluateAsync_ReturnsOnlyNewlyEarnedTitles()
    {
        var alreadyHeld = Guid.NewGuid();
        var newlyEarning = Guid.NewGuid();
        await SeedTitleAsync("already-held", TitleExclusivity.Shared, alreadyHeld);
        await SeedTitleAsync("newly-earned", TitleExclusivity.Shared);
        var evaluator = MakeEvaluator(
            new StubRule("already-held", TitleExclusivity.Shared, alreadyHeld),
            new StubRule("newly-earned", TitleExclusivity.Shared, newlyEarning));

        var earned = await evaluator.EvaluateAsync(CreateMatch());

        Assert.Single(earned);
        Assert.Equal("newly-earned", earned[0].RuleKey);
        Assert.Equal(newlyEarning, earned[0].HeroId);
    }

    [Fact]
    public async Task EvaluateAsync_SharedRule_NewQualifier_PersistsTheMetric()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("shared-rule", TitleExclusivity.Shared);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, new Dictionary<Guid, double?> { [heroId] = 214 }));

        await evaluator.EvaluateAsync(CreateMatch());

        var heroTitle = await HeroTitleAsync(titleId, heroId);
        Assert.Equal(214, heroTitle.Metric);
    }

    [Fact]
    public async Task EvaluateAsync_SharedRule_TwoNewQualifiers_ReturnsOneEarnedTitlePerHero()
    {
        var firstHero = Guid.NewGuid();
        var secondHero = Guid.NewGuid();
        await SeedTitleAsync("shared-rule", TitleExclusivity.Shared);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, firstHero, secondHero));

        var earned = await evaluator.EvaluateAsync(CreateMatch());

        Assert.Equal(new[] { firstHero, secondHero }.OrderBy(x => x), earned.Select(e => e.HeroId).OrderBy(x => x));
        Assert.All(earned, e => Assert.Equal("shared-rule", e.RuleKey));
    }

    [Fact]
    public async Task EvaluateAsync_NewQualifier_ReturnsTheMetricOnTheEarnedTitle()
    {
        var heroId = Guid.NewGuid();
        await SeedTitleAsync("shared-rule", TitleExclusivity.Shared);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, new Dictionary<Guid, double?> { [heroId] = 214 }));

        var earned = await evaluator.EvaluateAsync(CreateMatch());

        Assert.Equal(214, Assert.Single(earned).Metric);
    }

    [Fact]
    public async Task EvaluateAsync_SharedRule_ReQualifying_RefreshesTheMetricToTheLatestValue()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("shared-rule", TitleExclusivity.Shared, heroId);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, new Dictionary<Guid, double?> { [heroId] = 5 }));

        await evaluator.EvaluateAsync(CreateMatch());

        var heroTitle = await HeroTitleAsync(titleId, heroId);
        Assert.Equal(5, heroTitle.Metric);
    }

    [Fact]
    public async Task EvaluateAsync_UniqueRule_HolderUnchanged_MetricStillRefreshesToTheCurrentValue()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("unique-rule", TitleExclusivity.Unique, heroId);
        var evaluator = MakeEvaluator(new StubRule("unique-rule", TitleExclusivity.Unique, new Dictionary<Guid, double?> { [heroId] = 9 }));

        await evaluator.EvaluateAsync(CreateMatch());

        var heroTitle = await HeroTitleAsync(titleId, heroId);
        Assert.Equal(9, heroTitle.Metric);
    }

    [Fact]
    public async Task EvaluateAsync_RuleWithNoMetric_LeavesMetricNull()
    {
        var heroId = Guid.NewGuid();
        var titleId = await SeedTitleAsync("shared-rule", TitleExclusivity.Shared);
        var evaluator = MakeEvaluator(new StubRule("shared-rule", TitleExclusivity.Shared, heroId));

        await evaluator.EvaluateAsync(CreateMatch());

        var heroTitle = await HeroTitleAsync(titleId, heroId);
        Assert.Null(heroTitle.Metric);
    }

    [Fact]
    public async Task EvaluateAsync_RuleWithNoSeededTitleRow_IsSkippedWithoutThrowing()
    {
        var evaluator = MakeEvaluator(new StubRule("not-seeded", TitleExclusivity.Shared, Guid.NewGuid()));

        var earned = await evaluator.EvaluateAsync(CreateMatch());

        Assert.Empty(earned);
    }

    private TitleEvaluator MakeEvaluator(params ITitleRule[] rules)
        => new(_unitOfWork, rules);

    private async Task<Guid> SeedTitleAsync(string ruleKey, TitleExclusivity exclusivity, params Guid[] initialHolders)
    {
        var entity = new TitleEntity
        {
            Id = Guid.NewGuid(),
            Name = ruleKey,
            Comment = string.Empty,
            RuleKey = ruleKey,
            Exclusivity = exclusivity,
            HeroTitles = initialHolders.Select(id => new HeroTitleEntity { HeroesId = id }).ToList()
        };
        _dbContext.Titles.Add(entity);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();
        return entity.Id;
    }

    private async Task<List<Guid>> HolderIdsAsync(Guid titleId)
        => await _dbContext.HeroTitles.Where(ht => ht.TitlesId == titleId).Select(ht => ht.HeroesId).ToListAsync();

    private async Task<HeroTitleEntity> HeroTitleAsync(Guid titleId, Guid heroId)
        => await _dbContext.HeroTitles.AsNoTracking().SingleAsync(ht => ht.TitlesId == titleId && ht.HeroesId == heroId);

    private static MatchEntity CreateMatch(DateTime? date = null)
        => new() { Id = Guid.NewGuid(), Date = date ?? DateTime.UtcNow, Fighters = new List<FighterEntity>() };

    private class StubRule : ITitleRule
    {
        private readonly IReadOnlyDictionary<Guid, double?> _qualifiers;

        public StubRule(string ruleKey, TitleExclusivity exclusivity, params Guid[] qualifiers)
            : this(ruleKey, exclusivity, qualifiers.ToDictionary(id => id, _ => (double?)null))
        {
        }

        public StubRule(string ruleKey, TitleExclusivity exclusivity, IReadOnlyDictionary<Guid, double?> qualifiers)
        {
            RuleKey = ruleKey;
            Exclusivity = exclusivity;
            _qualifiers = qualifiers;
        }

        public string RuleKey { get; }

        public TitleExclusivity Exclusivity { get; }

        public Task<IReadOnlyDictionary<Guid, double?>> EvaluateAsync(MatchEntity match)
            => Task.FromResult(_qualifiers);
    }
}
