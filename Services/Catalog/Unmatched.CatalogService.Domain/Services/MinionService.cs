namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class MinionService(IUnitOfWork unitOfWork) : IMinionService
{
    public async Task<IEnumerable<Minion>> GetAllAsync()
    {
        return await unitOfWork.Minions.GetAsync();
    }

    public Task<Minion?> GetAsync(Guid id)
    {
        return unitOfWork.Minions.GetByIdAsync(id);
    }
}
