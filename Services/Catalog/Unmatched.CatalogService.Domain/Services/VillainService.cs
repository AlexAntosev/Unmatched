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

    public async Task<Villain?> UpdateImageAsync(Guid id, string imageFileName)
    {
        var villain = await unitOfWork.Villains.GetByIdAsync(id);
        if (villain is null)
        {
            return null;
        }

        villain.ImageFileName = imageFileName;
        unitOfWork.Villains.AddOrUpdate(villain, id);
        await unitOfWork.SaveChangesAsync();
        return villain;
    }
}
