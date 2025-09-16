namespace Unmatched.Services.Contracts;

using Unmatched.Dtos;

public interface IMapService
{
    Task<IEnumerable<MapDto>> GetAsync();
}
