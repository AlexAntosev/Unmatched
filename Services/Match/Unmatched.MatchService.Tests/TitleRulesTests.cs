namespace Unmatched.MatchService.Tests;

using Moq;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Titles.Rules;

public class TitleRulesTests
{
    private static readonly CatalogHeroDto ReferenceHero = new() { Hp = 16, DeckSize = 10, Sidekicks = [new CatalogSidekickDto { Hp = 3, Count = 1 }] };

    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMatchRepository> _matchRepository = new();
    private readonly Mock<IRatingRepository> _ratingRepository = new();
    private readonly Mock<ICatalogHeroCache> _catalogHeroCache = new();

    public TitleRulesTests()
    {
        _unitOfWork.Setup(u => u.Matches).Returns(_matchRepository.Object);
        _unitOfWork.Setup(u => u.Ratings).Returns(_ratingRepository.Object);
        _catalogHeroCache.Setup(c => c.GetAsync(It.IsAny<Guid>())).ReturnsAsync(ReferenceHero);
    }

    [Fact]
    public async Task Flawless_WinnerAtFullHp_Qualifies()
    {
        var winnerId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, hpLeft: ReferenceHero.Hp));

        var qualifiers = await new FlawlessTitleRule(_catalogHeroCache.Object).EvaluateAsync(match);

        Assert.Contains(winnerId, qualifiers.Keys);
        Assert.Null(qualifiers[winnerId]);
    }

    [Fact]
    public async Task Flawless_WinnerMissingASinglePoint_DoesNotQualify()
    {
        var winnerId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, hpLeft: ReferenceHero.Hp - 1));

        var qualifiers = await new FlawlessTitleRule(_catalogHeroCache.Object).EvaluateAsync(match);

        Assert.Empty(qualifiers);
    }

    [Fact]
    public async Task LastBreath_WinnerAtOneHp_Qualifies()
    {
        var winnerId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, hpLeft: 1));

        var qualifiers = await new LastBreathTitleRule().EvaluateAsync(match);

        Assert.Contains(winnerId, qualifiers.Keys);
    }

    [Fact]
    public async Task LastBreath_WinnerAtTwoHp_DoesNotQualify()
    {
        var winnerId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, hpLeft: 2));

        var qualifiers = await new LastBreathTitleRule().EvaluateAsync(match);

        Assert.Empty(qualifiers);
    }

    [Fact]
    public async Task LastBreath_WinnerAtThreeHp_DoesNotQualify()
    {
        var winnerId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, hpLeft: 3));

        var qualifiers = await new LastBreathTitleRule().EvaluateAsync(match);

        Assert.Empty(qualifiers);
    }

    [Fact]
    public async Task DeckMiller_WinnerWithZeroCards_Qualifies()
    {
        var winnerId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, cardsLeft: 0));

        var qualifiers = await new DeckMillerTitleRule().EvaluateAsync(match);

        Assert.Contains(winnerId, qualifiers.Keys);
        Assert.Null(qualifiers[winnerId]);
    }

    [Fact]
    public async Task GiantSlayer_BeatsAnOpponentRatedFarHigher_Qualifies()
    {
        var winnerId = Guid.NewGuid();
        var loserId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, matchPoints: 20), Fighter(loserId, isWinner: false, matchPoints: -20));
        // ratings shown are post-match; pre-match = post - matchPoints, so pre: winner=1000, loser=1300
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(winnerId)).ReturnsAsync(new RatingEntity { HeroId = winnerId, Points = 1020 });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(loserId)).ReturnsAsync(new RatingEntity { HeroId = loserId, Points = 1280 });

        var qualifiers = await new GiantSlayerTitleRule(_unitOfWork.Object).EvaluateAsync(match);

        Assert.Contains(winnerId, qualifiers.Keys);
        Assert.Equal(300, qualifiers[winnerId]); // 1300 - 1000
    }

    [Fact]
    public async Task GiantSlayer_OpponentBelowTheGapThreshold_DoesNotQualify()
    {
        var winnerId = Guid.NewGuid();
        var loserId = Guid.NewGuid();
        var match = Match(Fighter(winnerId, isWinner: true, matchPoints: 16), Fighter(loserId, isWinner: false, matchPoints: -16));
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(winnerId)).ReturnsAsync(new RatingEntity { HeroId = winnerId, Points = 1016 });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(loserId)).ReturnsAsync(new RatingEntity { HeroId = loserId, Points = 1184 }); // pre: 1000 vs 1200 - 200 gap

        var qualifiers = await new GiantSlayerTitleRule(_unitOfWork.Object).EvaluateAsync(match);

        Assert.Empty(qualifiers);
    }

    [Fact]
    public async Task GiantSlayer_MultipleQualifyingLosers_MetricIsTheLargestGap()
    {
        var winnerId = Guid.NewGuid();
        var closeLoserId = Guid.NewGuid();
        var giantLoserId = Guid.NewGuid();
        var match = Match(
            Fighter(winnerId, isWinner: true, matchPoints: 0),
            Fighter(closeLoserId, isWinner: false, matchPoints: 0),
            Fighter(giantLoserId, isWinner: false, matchPoints: 0));
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(winnerId)).ReturnsAsync(new RatingEntity { HeroId = winnerId, Points = 1000 });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(closeLoserId)).ReturnsAsync(new RatingEntity { HeroId = closeLoserId, Points = 1310 });
        _ratingRepository.Setup(r => r.GetByHeroIdAsync(giantLoserId)).ReturnsAsync(new RatingEntity { HeroId = giantLoserId, Points = 1450 });

        var qualifiers = await new GiantSlayerTitleRule(_unitOfWork.Object).EvaluateAsync(match);

        Assert.Equal(450, qualifiers[winnerId]);
    }

    [Fact]
    public async Task Streak_LongestWinStreak_IsFoundChronologically_RegardlessOfStorageOrder()
    {
        var streakyHero = Guid.NewGuid();
        var otherHero = Guid.NewGuid();
        // stored out of order on purpose - the rule must sort by Date itself
        var matches = new List<MatchEntity>
        {
            FinishedMatch(new DateTime(2026, 3, 1), Fighter(streakyHero, true), Fighter(otherHero, false)),
            FinishedMatch(new DateTime(2026, 1, 1), Fighter(streakyHero, true), Fighter(otherHero, false)),
            FinishedMatch(new DateTime(2026, 4, 1), Fighter(streakyHero, false), Fighter(otherHero, true)), // breaks the streak
            FinishedMatch(new DateTime(2026, 2, 1), Fighter(streakyHero, true), Fighter(otherHero, false)),
        };
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(matches);

        var holders = await new StreakTitleRule(_unitOfWork.Object).EvaluateAsync(Match());

        Assert.Equal(new[] { streakyHero }, holders.Keys);
        Assert.Equal(3, holders[streakyHero]); // three wins before the streak-breaking loss
    }

    [Fact]
    public async Task Sufferer_LongestLossStreak_IsTheMirrorOfStreak()
    {
        var unluckyHero = Guid.NewGuid();
        var otherHero = Guid.NewGuid();
        var matches = new List<MatchEntity>
        {
            FinishedMatch(new DateTime(2026, 1, 1), Fighter(unluckyHero, false), Fighter(otherHero, true)),
            FinishedMatch(new DateTime(2026, 2, 1), Fighter(unluckyHero, false), Fighter(otherHero, true)),
            FinishedMatch(new DateTime(2026, 3, 1), Fighter(unluckyHero, true), Fighter(otherHero, false)),
        };
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(matches);

        var holders = await new SuffererTitleRule(_unitOfWork.Object).EvaluateAsync(Match());

        Assert.Equal(new[] { unluckyHero }, holders.Keys);
        Assert.Equal(2, holders[unluckyHero]); // two losses before the streak-breaking win
    }

    [Fact]
    public async Task GrandChampion_HighestRatedHero_HoldsTheTitle()
    {
        var championId = Guid.NewGuid();
        _ratingRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<RatingEntity>
        {
            new() { HeroId = championId, Points = 1300 },
            new() { HeroId = Guid.NewGuid(), Points = 1100 },
        });

        var holders = await new GrandChampionTitleRule(_unitOfWork.Object).EvaluateAsync(Match());

        Assert.Equal(new[] { championId }, holders.Keys);
        Assert.Equal(1300, holders[championId]);
    }

    [Fact]
    public async Task Kingslayer_LastHeroToBeatTheChampion_HoldsTheTitle()
    {
        var championId = Guid.NewGuid();
        var earlierSlayer = Guid.NewGuid();
        var latestSlayer = Guid.NewGuid();
        _ratingRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<RatingEntity> { new() { HeroId = championId, Points = 1300 } });
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>
        {
            FinishedMatch(new DateTime(2026, 1, 1), Fighter(earlierSlayer, true), Fighter(championId, false)),
            FinishedMatch(new DateTime(2026, 2, 1), Fighter(championId, true), Fighter(earlierSlayer, false)),
            FinishedMatch(new DateTime(2026, 3, 1), Fighter(latestSlayer, true), Fighter(championId, false)),
        });

        var holders = await new KingslayerTitleRule(_unitOfWork.Object).EvaluateAsync(Match());

        Assert.Equal(new[] { latestSlayer }, holders.Keys);
        Assert.Null(holders[latestSlayer]);
    }

    [Fact]
    public async Task Kingslayer_ChampionHasNeverLost_TitleIsVacant()
    {
        var championId = Guid.NewGuid();
        _ratingRepository.Setup(r => r.GetAsync()).ReturnsAsync(new List<RatingEntity> { new() { HeroId = championId, Points = 1300 } });
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>
        {
            FinishedMatch(new DateTime(2026, 1, 1), Fighter(championId, true), Fighter(Guid.NewGuid(), false)),
        });

        var holders = await new KingslayerTitleRule(_unitOfWork.Object).EvaluateAsync(Match());

        Assert.Empty(holders);
    }

    [Fact]
    public async Task Wall_LowestAverageHpLostPerWin_HoldsTheTitle()
    {
        var carefulHero = Guid.NewGuid();
        var recklessHero = Guid.NewGuid();
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>
        {
            FinishedMatch(new DateTime(2026, 1, 1), Fighter(carefulHero, true, hpLeft: 15), Fighter(Guid.NewGuid(), false)), // lost 1
            FinishedMatch(new DateTime(2026, 2, 1), Fighter(recklessHero, true, hpLeft: 2), Fighter(Guid.NewGuid(), false)), // lost 14
        });

        var holders = await new WallTitleRule(_unitOfWork.Object, _catalogHeroCache.Object).EvaluateAsync(Match());

        Assert.Equal(new[] { carefulHero }, holders.Keys);
        Assert.Equal(1, holders[carefulHero]); // 1 HP lost / 1 win
    }

    [Fact]
    public async Task Workhorse_MostRankedMatchesPlayed_HoldsTheTitle()
    {
        var busyHero = Guid.NewGuid();
        var quietHero = Guid.NewGuid();
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>
        {
            RankedMatch(true, Fighter(busyHero, true), Fighter(Guid.NewGuid(), false)),
            RankedMatch(true, Fighter(busyHero, false), Fighter(Guid.NewGuid(), true)),
            RankedMatch(true, Fighter(quietHero, true), Fighter(Guid.NewGuid(), false)),
            RankedMatch(false, Fighter(busyHero, true), Fighter(quietHero, false)), // unranked - doesn't count for either
        });

        var holders = await new WorkhorseTitleRule(_unitOfWork.Object).EvaluateAsync(Match());

        Assert.Equal(new[] { busyHero }, holders.Keys);
        Assert.Equal(2, holders[busyHero]); // 2 ranked matches; the unranked one doesn't count
    }

    [Fact]
    public async Task Executioner_MostSidekickHpDestroyed_HoldsTheTitle()
    {
        // ReferenceHero has one sidekick with 3 HP total. dealerId's own sidekick survives untouched
        // (3 left); victimId's is fully destroyed (0 left) - that destruction is credited to dealerId,
        // the fighter on the OTHER side from whoever actually lost the sidekick HP.
        var dealerId = Guid.NewGuid();
        var victimId = Guid.NewGuid();
        _matchRepository.Setup(r => r.GetFinishedForRatingReplayAsync()).ReturnsAsync(new List<MatchEntity>
        {
            FinishedMatch(new DateTime(2026, 1, 1), Fighter(dealerId, true, sidekickHpLeft: 3), Fighter(victimId, false, sidekickHpLeft: 0)),
        });

        var holders = await new ExecutionerTitleRule(_unitOfWork.Object, _catalogHeroCache.Object).EvaluateAsync(Match());

        Assert.Equal(new[] { dealerId }, holders.Keys);
        Assert.Equal(3, holders[dealerId]); // ReferenceHero's sidekick has 3 max HP, all destroyed
    }

    private static FighterEntity Fighter(
        Guid heroId, bool isWinner, int? cardsLeft = null, int? matchPoints = null, int? hpLeft = null, int? sidekickHpLeft = null)
        => new()
        {
            HeroId = heroId,
            IsWinner = isWinner,
            CardsLeft = cardsLeft,
            MatchPoints = matchPoints,
            HpLeft = hpLeft,
            SidekickHpLeft = sidekickHpLeft
        };

    private static MatchEntity Match(params FighterEntity[] fighters)
        => new() { Fighters = fighters.ToList() };

    private static MatchEntity FinishedMatch(DateTime date, params FighterEntity[] fighters)
        => new() { Date = date, IsPlanned = false, Fighters = fighters.ToList() };

    private static MatchEntity RankedMatch(bool isRanked, params FighterEntity[] fighters)
        => new() { IsPlanned = false, IsRanked = isRanked, Fighters = fighters.ToList() };
}
