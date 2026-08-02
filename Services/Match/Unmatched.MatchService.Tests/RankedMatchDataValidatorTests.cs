namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Validation;

public class RankedMatchDataValidatorTests
{
    private static readonly Guid HeroWithSidekickId = Guid.NewGuid();
    private static readonly Guid HeroWithoutSidekickId = Guid.NewGuid();

    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();
    private readonly RankedMatchDataValidator _validator;

    public RankedMatchDataValidatorTests()
    {
        _catalogHeroCache.Setup(c => c.GetAsync(HeroWithSidekickId))
            .ReturnsAsync(new CatalogHeroDto { Id = HeroWithSidekickId, Hp = 16, DeckSize = 10, Sidekicks = new[] { new CatalogSidekickDto { Hp = 3, Count = 1 } } });
        _catalogHeroCache.Setup(c => c.GetAsync(HeroWithoutSidekickId))
            .ReturnsAsync(new CatalogHeroDto { Id = HeroWithoutSidekickId, Hp = 16, DeckSize = 10, Sidekicks = Array.Empty<CatalogSidekickDto>() });

        _validator = new RankedMatchDataValidator(_catalogHeroCache.Object);
    }

    [Fact]
    public async Task ValidateAsync_RankedWithMissingHp_Throws()
    {
        var match = CreateMatch(isRanked: true, hpLeft: null, cardsLeft: 5, sidekickHpLeft: null, heroId: HeroWithoutSidekickId);

        await Assert.ThrowsAsync<ArgumentException>(() => _validator.ValidateAsync(match));
    }

    [Fact]
    public async Task ValidateAsync_RankedWithMissingCards_Throws()
    {
        var match = CreateMatch(isRanked: true, hpLeft: 10, cardsLeft: null, sidekickHpLeft: null, heroId: HeroWithoutSidekickId);

        await Assert.ThrowsAsync<ArgumentException>(() => _validator.ValidateAsync(match));
    }

    [Fact]
    public async Task ValidateAsync_RankedHeroWithSidekickMissingSidekickHp_Throws()
    {
        var match = CreateMatch(isRanked: true, hpLeft: 10, cardsLeft: 5, sidekickHpLeft: null, heroId: HeroWithSidekickId);

        await Assert.ThrowsAsync<ArgumentException>(() => _validator.ValidateAsync(match));
    }

    [Fact]
    public async Task ValidateAsync_RankedHeroWithoutASidekick_DoesNotRequireSidekickHp()
    {
        var match = CreateMatch(isRanked: true, hpLeft: 10, cardsLeft: 5, sidekickHpLeft: null, heroId: HeroWithoutSidekickId);

        await _validator.ValidateAsync(match); // does not throw
    }

    [Fact]
    public async Task ValidateAsync_Unranked_AllowsEveryFieldToBeNull()
    {
        var match = CreateMatch(isRanked: false, hpLeft: null, cardsLeft: null, sidekickHpLeft: null, heroId: HeroWithSidekickId);

        await _validator.ValidateAsync(match); // does not throw
    }

    [Fact]
    public async Task ValidateAsync_UnrankedButBelongsToATournament_StillRequiresCompleteData()
    {
        var match = CreateMatch(isRanked: false, hpLeft: null, cardsLeft: 5, sidekickHpLeft: null, heroId: HeroWithoutSidekickId);
        match.TournamentId = Guid.NewGuid();

        await Assert.ThrowsAsync<ArgumentException>(() => _validator.ValidateAsync(match));
    }

    [Fact]
    public async Task ValidateAsync_Cooperative_IsAlwaysSkipped_EvenWhenRankedWithMissingData()
    {
        var match = new MatchEntity
        {
            GameMode = GameMode.Cooperative,
            IsRanked = true,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = HeroWithSidekickId, HpLeft = null, CardsLeft = null, SidekickHpLeft = null },
            },
        };

        await _validator.ValidateAsync(match); // does not throw
    }

    private static MatchEntity CreateMatch(bool isRanked, int? hpLeft, int? cardsLeft, int? sidekickHpLeft, Guid heroId)
        => new()
        {
            GameMode = GameMode.OneVsOne,
            IsRanked = isRanked,
            Fighters = new List<FighterEntity>
            {
                new() { HeroId = heroId, IsWinner = true, HpLeft = hpLeft, CardsLeft = cardsLeft, SidekickHpLeft = sidekickHpLeft },
            },
        };
}
