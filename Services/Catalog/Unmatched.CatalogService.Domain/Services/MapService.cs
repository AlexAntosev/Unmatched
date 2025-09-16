namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class MapService(IUnitOfWork unitOfWork) : IMapService
{

    public async Task<IEnumerable<Map>> GetAllAsync()
    {
        return await unitOfWork.Maps.GetAsync();
    }

    public Task<Map?> GetAsync(Guid id)
    {
        return unitOfWork.Maps.GetByIdAsync(id);
    }
}
