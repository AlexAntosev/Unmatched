namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class VillainStatsCoordinator(
    ILoggerFactory loggerFactory,
    IMapper mapper,
    ICatalogVillainCache catalogVillainCache) : IStatsCoordinator
{
    public IUnitOfWork UnitOfWork { get; set; }

    private ILogger<VillainStatsCoordinator> Logger { get; } = loggerFactory.CreateLogger<VillainStatsCoordinator>();

    public async Task CheckAndInitializeAsync()
    {
        Logger.LogInformation("Some villain data exists. Checking for new villains...");
        var statsToAdd = new List<VillainStats>();
        var villains = await catalogVillainCache.GetAsync();
        foreach (var villain in villains)
        {
            var existingStats = await UnitOfWork.VillainStats.GetAsync(villain.Id);
            if (existingStats == null)
            {
                var freshStats = mapper.Map<VillainStats>(villain);
                freshStats.ModifiedAt = DateTime.UtcNow;
                statsToAdd.Add(freshStats);

                Logger.LogInformation("Adding '{VillainName}' villain...", villain.Name);
            }
        }

        if (statsToAdd.Any())
        {
            await UnitOfWork.VillainStats.AddRangeAsync(statsToAdd);
        }
    }

    public Task<bool> HasDataAsync()
    {
        return UnitOfWork.VillainStats.HasRecordsAsync();
    }

    public async Task InitializeAsync(IReadOnlyCollection<MatchDto> matches)
    {
        Logger.LogInformation("Data initialization requested.");
        Logger.LogInformation("Clearing Villain statistics...");
        await UnitOfWork.VillainStats.DeleteAllAsync();

        var villains = await catalogVillainCache.GetAsync();
        var orderedMatches = matches.OrderByDescending(x => x.Date);

        var statistics = new List<VillainStats>();

        foreach (var villain in villains)
        {
            var villainMatches = orderedMatches.Where(x => x.Villain != null && x.Villain.VillainId == villain.Id).ToList();
            var stat = mapper.Map<VillainStats>(villain);

            stat.TotalMatches = villainMatches.Count;
            stat.TotalWins = villainMatches.Count(x => x.Villain!.IsWinner);
            stat.TotalLooses = villainMatches.Count(x => x.Villain!.IsWinner == false);
            stat.LastMatchIncludedAt = villainMatches.FirstOrDefault()?.Date ?? DateTime.MinValue;
            stat.ModifiedAt = DateTime.UtcNow;

            statistics.Add(stat);
        }

        Logger.LogInformation("Filling fresh Villain statistics...");

        await UnitOfWork.VillainStats.AddRangeAsync(statistics);

        Logger.LogInformation("Villain stats initialization finished.");
    }
}
