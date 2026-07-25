namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class OwnedExpansionService(IUnitOfWork unitOfWork) : IOwnedExpansionService
{
    public async Task<IEnumerable<Guid>> GetOwnedExpansionIdsAsync()
    {
        var owned = await unitOfWork.OwnedExpansions.GetAsync();
        return owned.Select(o => o.ExpansionId);
    }

    public async Task ReplaceAsync(IEnumerable<Guid> expansionIds)
    {
        unitOfWork.OwnedExpansions.DeleteAll();
        await unitOfWork.OwnedExpansions.AddRangeAsync(expansionIds.Select(id => new OwnedExpansion { ExpansionId = id }));
        await unitOfWork.SaveChangesAsync();
    }
}
