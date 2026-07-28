namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IMapService
{
    Task<IEnumerable<MapDto>> GetAsync();

    Task<string> UpdateImageAsync(Guid mapId, string imageFileName);
}
