namespace Unmatched.StatisticsService.Domain.Initialize;

using Microsoft.Extensions.Logging;

using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Repositories;

public class StatisticsInitializer(ILogger<StatisticsInitializer> logger,IUnitOfWork unitOfWork, IEnumerable<IStatsCoordinator> coordinators, IMatchClient matchClient) : IStatisticsInitializer
{
    public async Task InitializeAsync()
    {
        try
        {
            foreach (var coordinator in coordinators)
            {
                coordinator.UnitOfWork = unitOfWork;
                if (await coordinator.HasDataAsync() == false)
                {
                    // IDEA: here we query all matches. But in case of huge amount of data this should be improved.
                    var matches = await GetAllMatches();
                    await coordinator.InitializeAsync(matches);
                }
                else
                {
                    await coordinator.CheckAndInitializeAsync();
                }
            }

            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize stats data.");
        }
    }

    /// <remarks>
    /// Unlike <see cref="InitializeAsync"/>, this is a deliberate, user-triggered action (following a
    /// rating recalculation), so a failure here is left to propagate rather than logged and swallowed -
    /// the caller needs to know the rebuild didn't happen.
    /// </remarks>
    public async Task RebuildAsync()
    {
        var matches = await GetAllMatches();
        foreach (var coordinator in coordinators)
        {
            coordinator.UnitOfWork = unitOfWork;
            await coordinator.InitializeAsync(matches);
        }

        await unitOfWork.SaveChangesAsync();
    }

    private async Task<IReadOnlyList<MatchDto>> GetAllMatches()
    {
        if (_matchCache.Any() == false)
        {
            // Planned matches have no result yet - counting them would inflate TotalMatches/TotalLooses
            // for every coordinator that reads this list.
            _matchCache = (await matchClient.GetAllMatchesAsync()).Where(m => !m.IsPlanned).ToList();
        }

        return _matchCache;
    }

    private IReadOnlyList<MatchDto> _matchCache = new List<MatchDto>();
}