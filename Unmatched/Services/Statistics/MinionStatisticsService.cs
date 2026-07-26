namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class MinionStatisticsService(IStatisticsClient statisticsClient) : IMinionStatisticsService
{
    public async Task<IEnumerable<MinionStatisticsDto>> GetMinionsStatisticsAsync()
    {
        return await statisticsClient.GetMinionStatsAsync();
    }

    public async Task<MinionStatisticsDto> GetMinionStatisticsAsync(Guid minionId)
    {
        return await statisticsClient.GetMinionStatsAsync(minionId);
    }

    public async Task<string> UpdateImageAsync(Guid minionId, string imageFileName)
    {
        var updated = await statisticsClient.UpdateMinionImageAsync(minionId, imageFileName);
        return updated.ImageFileName!;
    }
}
