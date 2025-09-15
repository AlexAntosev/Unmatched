namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class PlayerStatisticsService(IMapper mapper, IMatchClient matchClient) : IPlayerStatisticsService
{
    public Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UiMatchLogDto>> GetPlayerMatchesAsync(Guid playerId)
    {
        var matches = await matchClient.GetFinishedByPlayerAsync(playerId);
        return matches.Select(mapper.Map<UiMatchLogDto>);
    }
}
