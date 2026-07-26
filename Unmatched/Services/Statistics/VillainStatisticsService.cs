namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class VillainStatisticsService(IStatisticsClient statisticsClient) : IVillainStatisticsService
{
    public async Task<IEnumerable<VillainStatisticsDto>> GetVillainsStatisticsAsync()
    {
        return await statisticsClient.GetVillainStatsAsync();
    }

    public async Task<VillainStatisticsDto> GetVillainStatisticsAsync(Guid villainId)
    {
        return await statisticsClient.GetVillainStatsAsync(villainId);
    }
}
