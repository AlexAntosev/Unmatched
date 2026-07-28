namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class MapService(IMapper mapper, ICatalogClient catalogClient) : IMapService
{
    public async Task<IEnumerable<MapDto>> GetAsync()
    {
        var entities = await catalogClient.GetMapsAsync();
        var maps = mapper.Map<IEnumerable<MapDto>>(entities);
        return maps;
    }

    public async Task<string> UpdateImageAsync(Guid mapId, string imageFileName)
    {
        var updated = await catalogClient.UpdateMapImageAsync(mapId, imageFileName);
        return updated.ImageFileName!;
    }
}
