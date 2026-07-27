namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class ExpansionService(IUnitOfWork unitOfWork) : IExpansionService
{
    public async Task<IEnumerable<Expansion>> GetAllAsync()
    {
        return await unitOfWork.Expansions.GetAsync();
    }

    public async Task<Expansion?> UpdateImageAsync(Guid id, string imageFileName)
    {
        var expansion = await unitOfWork.Expansions.GetByIdAsync(id);
        if (expansion is null)
        {
            return null;
        }

        expansion.ImageFileName = imageFileName;
        unitOfWork.Expansions.AddOrUpdate(expansion, id);
        await unitOfWork.SaveChangesAsync();
        return expansion;
    }
}
