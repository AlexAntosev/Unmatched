namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class HeroStatsCoordinator(
    ILoggerFactory loggerFactory,
    IMapper mapper,
    ICatalogHeroCache catalogHeroCache,
    IMatchClient matchClient,
    IHeroPlaceAdjuster heroPlaceAdjuster) : IStatsCoordinator
{
    public IUnitOfWork UnitOfWork { get; set; }

    private ILogger<HeroStatsCoordinator> Logger { get; } = loggerFactory.CreateLogger<HeroStatsCoordinator>();

    public async Task CheckAndInitializeAsync()
    {

        Logger.LogInformation("Some hero data exists. Checking for new heroes...");
        var statsToAdd = new List<HeroStats>();

        var allStats = (await UnitOfWork.HeroStats.GetAllAsync()).ToList();

        var heroes = await catalogHeroCache.GetAsync();
        foreach (var hero in heroes)
        {
            var existingStats =await UnitOfWork.HeroStats.GetAsync(hero.Id);
            if (allStats.Any(x => x.HeroId == hero.Id) == false)
            {
                var freshStats = mapper.Map<HeroStats>(hero);
                freshStats.ModifiedAt = DateTime.UtcNow;
                statsToAdd.Add(freshStats);

                Logger.LogInformation("Adding '{HeroName}' hero...", hero.Name);
            }
        }

        if (statsToAdd.Any())
        {
            Logger.LogInformation("Adding {count} heroes ...", statsToAdd.Count);
            allStats.AddRange(statsToAdd);

            Logger.LogInformation("Recalculating hero places ...");
            allStats = heroPlaceAdjuster.Adjust(allStats).ToList();
            await UnitOfWork.HeroStats.AddOrUpdateAsync(allStats);
        }
    }

    public Task<bool> HasDataAsync()
    {
        return UnitOfWork.HeroStats.HasRecordsAsync();
    }

    public async Task InitializeAsync(IReadOnlyCollection<MatchDto> matches)
    {
        Logger.LogInformation("Data initialization requested.");
        Logger.LogInformation("Clearing Hero statistics...");
        await UnitOfWork.HeroStats.DeleteAllAsync();

        var heroes = await catalogHeroCache.GetAsync();
        var ratings = await matchClient.GetAllRatingsAsync();
        var orderedMatches = matches.OrderByDescending(x => x.Date);
        var fighters = orderedMatches.SelectMany(x => x.Fighters);

        var statistics = new List<HeroStats>();

        foreach (var hero in heroes)
        {
            var points = ratings.FirstOrDefault(x => x.HeroId.Equals(hero.Id))?.Points ?? 0;
            var fights = fighters.Where(x => x.HeroId.Equals(hero.Id)).ToArray();

            var heroStatistics = mapper.Map<HeroStats>(hero);
            heroStatistics.Points = points;
            heroStatistics.TotalMatches = fights.Count();
            heroStatistics.TotalWins = fights.Count(x => x.IsWinner);
            heroStatistics.TotalLooses = fights.Count(x => x.IsWinner == false);
            heroStatistics.LastMatchPoints = fights.FirstOrDefault()?.MatchPoints ?? 0;
            heroStatistics.LastMatchIncludedAt = orderedMatches.FirstOrDefault(m => m.Fighters.Any(f => f.HeroId.Equals(hero.Id)))?.Date ?? DateTime.MinValue;
            heroStatistics.ModifiedAt = DateTime.UtcNow;

            statistics.Add(heroStatistics);
        }

        statistics = heroPlaceAdjuster.Adjust(statistics).ToList();

        Logger.LogInformation("Filling fresh Hero statistics...");

        await UnitOfWork.HeroStats.AddRangeAsync(statistics);

        Logger.LogInformation("Hero stats initialization finished.");
    }
}