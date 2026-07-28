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

    public async Task<Map?> UpdateImageAsync(Guid id, string imageFileName)
    {
        var map = await unitOfWork.Maps.GetByIdAsync(id);
        if (map is null)
        {
            return null;
        }

        map.ImageFileName = imageFileName;
        unitOfWork.Maps.AddOrUpdate(map, id);
        await unitOfWork.SaveChangesAsync();
        return map;
    }
}
