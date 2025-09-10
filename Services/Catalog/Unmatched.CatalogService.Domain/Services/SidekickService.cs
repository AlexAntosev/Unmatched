namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class SidekickService(IUnitOfWork unitOfWork) : ISidekickService
{
    public async Task<IEnumerable<Sidekick>> GetAsync()
    {
        return await unitOfWork.Sidekicks.GetAsync();
    }

    public async Task<IEnumerable<Sidekick>> GetByHeroAsync(Guid heroId)
    {
        return await unitOfWork.Sidekicks.GetByHeroAsync(heroId);
    }
}
