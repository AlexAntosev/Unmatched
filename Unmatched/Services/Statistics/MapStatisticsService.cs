namespace Unmatched.Services.Statistics;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class MapStatisticsService(IMapper mapper, IMatchClient matchClient, IStatisticsClient statisticsClient) : IMapStatisticsService
{
    public async Task<IEnumerable<UiMatchLogDto>> GetMapMatchesAsync(Guid mapId)
    {
        var matches = await matchClient.GetFinishedByMapAsync(mapId);
        return matches.Select(mapper.Map<UiMatchLogDto>);
    }

    public async Task<IEnumerable<UiMapStatisticsDto>> GetMapsStatisticsAsync()
    {
        var dtos = await statisticsClient.GetMapsStatisticsAsync();
        return mapper.Map<IEnumerable<UiMapStatisticsDto>>(dtos);
    }

    public async Task<UiMapStatisticsDto> GetMapStatisticsAsync(Guid mapId)
    {
        var dto = await statisticsClient.GetMapsStatisticsAsync(mapId);
        return mapper.Map<UiMapStatisticsDto>(dto);
    }
}
