namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class VillainService(IUnitOfWork unitOfWork) : IVillainService
{
    public async Task<IEnumerable<Villain>> GetAllAsync()
    {
        return await unitOfWork.Villains.GetAsync();
    }

    public Task<Villain?> GetAsync(Guid id)
    {
        return unitOfWork.Villains.GetByIdAsync(id);
    }
}
