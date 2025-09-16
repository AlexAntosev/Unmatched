namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;

public interface IMapService
{
    Task<IEnumerable<Map>> GetAllAsync();

    Task<Map?> GetAsync(Guid id);
}
