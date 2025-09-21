namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class MapStatsCoordinator(
    ILoggerFactory loggerFactory,
    IMapper mapper,
    ICatalogMapCache catalogMapCache,
    IMatchClient matchClient) : IStatsCoordinator
{
    public IUnitOfWork UnitOfWork { get; set; }

    private ILogger<MapStatsCoordinator> Logger { get; } = loggerFactory.CreateLogger<MapStatsCoordinator>();

    public async Task CheckAndInitializeAsync()
    {
        Logger.LogInformation("Some map data exists. Checking for new maps...");
        var statsToAdd = new List<MapStats>();
        var maps = await catalogMapCache.GetAsync();
        foreach (var map in maps)
        {
            var existingStats = await UnitOfWork.MapStats.GetAsync(map.Id);
            if (existingStats == null)
            {
                var freshStats = mapper.Map<MapStats>(map);
                freshStats.ModifiedAt = DateTime.UtcNow;
                statsToAdd.Add(freshStats);

                Logger.LogInformation("Adding '{MapName}' map...", map.Name);
            }
        }

        if (statsToAdd.Any())
        {
            await UnitOfWork.MapStats.AddRangeAsync(statsToAdd);
        }
    }

    public Task<bool> HasDataAsync()
    {
        return UnitOfWork.MapStats.HasRecordsAsync();
    }

    public async Task InitializeAsync(IReadOnlyCollection<MatchDto> matches)
    {
        Logger.LogInformation("Data initialization requested.");
        Logger.LogInformation("Clearing Map statistics...");
        await UnitOfWork.HeroStats.DeleteAllAsync();

        var maps = await catalogMapCache.GetAsync();
        var orderedMatches = matches.OrderByDescending(x => x.Date);

        var statistics = new List<MapStats>();

        foreach (var map in maps)
        {
            var mapMatches = orderedMatches.Where(x => (x.Map != null) && (x.Map.Id == map.Id));
            var stat = mapper.Map<MapStats>(map);

            stat.TotalMatches = mapMatches.Count();
            stat.LastMatchIncludedAt = mapMatches.FirstOrDefault(m => m.Map!.Id.Equals(map.Id))?.Date ?? DateTime.MinValue;
            stat.ModifiedAt = DateTime.UtcNow;

            statistics.Add(stat);
        }
        
        Logger.LogInformation("Filling fresh Map statistics...");

        await UnitOfWork.MapStats.AddRangeAsync(statistics);

        Logger.LogInformation("Map stats initialization finished.");
    }
}
