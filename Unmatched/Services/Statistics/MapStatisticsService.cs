namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class MapStatisticsService(IMapper mapper, IMatchClient matchClient) : IMapStatisticsService
{
    public Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<MapStatisticsDto> GetMapStatisticsAsync(Guid mapId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UiMatchLogDto>> GetMapMatchesAsync(Guid mapId)
    {
        var matches = await matchClient.GetFinishedByMapAsync(mapId);
        return matches.Select(mapper.Map<UiMatchLogDto>);
    }
}
