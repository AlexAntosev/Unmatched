namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.HttpClients.Contracts;

public class VillainStatisticsService(IMapper mapper, IMatchClient matchClient, IStatisticsClient statisticsClient) : IVillainStatisticsService
{
    public async Task<IEnumerable<VillainStatisticsDto>> GetVillainsStatisticsAsync()
    {
        return await statisticsClient.GetVillainStatsAsync();
    }

    public async Task<VillainStatisticsDto> GetVillainStatisticsAsync(Guid villainId)
    {
        return await statisticsClient.GetVillainStatsAsync(villainId);
    }

    public async Task<string> UpdateImageAsync(Guid villainId, string imageFileName)
    {
        var updated = await statisticsClient.UpdateVillainImageAsync(villainId, imageFileName);
        return updated.ImageFileName!;
    }

    public async Task<IEnumerable<UiMatchLogDto>> GetVillainMatchesAsync(Guid villainId)
    {
        var matches = await matchClient.GetFinishedByVillainAsync(villainId);
        return matches.Select(mapper.Map<UiMatchLogDto>);
    }
}
