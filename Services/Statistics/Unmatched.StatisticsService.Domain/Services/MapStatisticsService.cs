namespace Unmatched.StatisticsService.Domain.Services;

using AutoMapper;
using Unmatched.StatisticsService.Domain.Communication.Catalog.Http;
using Unmatched.StatisticsService.Domain.Communication.Match.Http;
using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Services.Contracts;

public class MapStatisticsService(IMapper mapper, ICatalogMapCache catalogMapCache, IMatchClient matchClient) : IMapStatisticsService
{
    public async Task<IEnumerable<MapStats>> GetMapsStatisticsAsync()
    {
        var catalogMaps = await catalogMapCache.GetAsync();
        var maps = mapper.Map<List<MapStats>>(catalogMaps);
        var mapMatches = await matchClient.GetAllMatchesAsync();

        foreach (var map in maps)
        {
            var matchCount = mapMatches.Where(x => x.Map != null).Count(x => x.Map!.Id.Equals(map.MapId));

            map.TotalMatches = matchCount;
        }

        return maps;
    }

    public async Task<MapStats> GetMapStatisticsAsync(Guid mapId)
    {
        var mapEntity = await catalogMapCache.GetAsync(mapId);
        var map = mapper.Map<MapStats>(mapEntity);
        var mapMatches = await matchClient.GetAllMatchesAsync();

        map.TotalMatches = mapMatches.Count();
        return map;
    }
}
