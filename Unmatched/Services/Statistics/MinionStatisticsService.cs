namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;
using Unmatched.HttpClients.Contracts;

public class MinionStatisticsService(IMapper mapper, IMatchClient matchClient, IStatisticsClient statisticsClient) : IMinionStatisticsService
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

    public async Task<IEnumerable<UiMatchLogDto>> GetMinionMatchesAsync(Guid minionId)
    {
        var matches = await matchClient.GetFinishedByMinionAsync(minionId);
        return matches.Select(mapper.Map<UiMatchLogDto>);
    }
}
