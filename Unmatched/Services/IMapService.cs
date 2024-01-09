namespace Unmatched.Services;

using Unmatched.Dtos;

public interface IMapService
{
    Task<IEnumerable<MapDto>> GetAsync();
}
