namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class PlayerStatisticsService(IMapper mapper, IMatchClient matchClient, IStatisticsClient statisticsClient) : IPlayerStatisticsService
{
    public Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync()
    {
        return statisticsClient.GetPlayerStatsAsync();
    }

    public Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId)
    {
        return statisticsClient.GetPlayerStatsAsync(playerId);
    }

    public async Task<IEnumerable<UiMatchLogDto>> GetPlayerMatchesAsync(Guid playerId)
    {
        var matches = await matchClient.GetFinishedByPlayerAsync(playerId);
        return matches.Select(mapper.Map<UiMatchLogDto>);
    }
}
