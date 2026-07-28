namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;

public class MinionStatsCoordinator(
    ILoggerFactory loggerFactory,
    IMapper mapper,
    ICatalogMinionCache catalogMinionCache) : IStatsCoordinator
{
    public IUnitOfWork UnitOfWork { get; set; }

    private ILogger<MinionStatsCoordinator> Logger { get; } = loggerFactory.CreateLogger<MinionStatsCoordinator>();

    public async Task CheckAndInitializeAsync()
    {
        Logger.LogInformation("Some minion data exists. Checking for new minions...");
        var statsToAdd = new List<MinionStats>();
        var minions = await catalogMinionCache.GetAsync();
        foreach (var minion in minions)
        {
            var existingStats = await UnitOfWork.MinionStats.GetAsync(minion.Id);
            if (existingStats == null)
            {
                var freshStats = mapper.Map<MinionStats>(minion);
                freshStats.ModifiedAt = DateTime.UtcNow;
                statsToAdd.Add(freshStats);

                Logger.LogInformation("Adding '{MinionName}' minion...", minion.Name);
            }
        }

        if (statsToAdd.Any())
        {
            await UnitOfWork.MinionStats.AddRangeAsync(statsToAdd);
        }
    }

    public Task<bool> HasDataAsync()
    {
        return UnitOfWork.MinionStats.HasRecordsAsync();
    }

    public async Task InitializeAsync(IReadOnlyCollection<MatchDto> matches)
    {
        Logger.LogInformation("Data initialization requested.");
        Logger.LogInformation("Clearing Minion statistics...");
        await UnitOfWork.MinionStats.DeleteAllAsync();

        var minions = await catalogMinionCache.GetAsync();
        var orderedMatches = matches.OrderByDescending(x => x.Date).ToList();

        var statistics = new List<MinionStats>();

        foreach (var minion in minions)
        {
            var matchesWithMinion = orderedMatches
                .Where(x => x.Villain != null && x.Villain.Minions.Any(m => m.MinionId == minion.Id))
                .ToList();
            var minionEntries = matchesWithMinion
                .Select(x => x.Villain!.Minions.First(m => m.MinionId == minion.Id))
                .ToList();

            var stat = mapper.Map<MinionStats>(minion);
            stat.TotalMatches = matchesWithMinion.Count;
            stat.TotalWins = minionEntries.Count(m => m.IsWinner);
            stat.TotalLooses = minionEntries.Count(m => m.IsWinner == false);
            stat.LastMatchIncludedAt = matchesWithMinion.FirstOrDefault()?.Date ?? DateTime.MinValue;
            stat.ModifiedAt = DateTime.UtcNow;

            statistics.Add(stat);
        }

        Logger.LogInformation("Filling fresh Minion statistics...");

        await UnitOfWork.MinionStats.AddRangeAsync(statistics);

        Logger.LogInformation("Minion stats initialization finished.");
    }
}
