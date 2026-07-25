namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class ExpansionService(IUnitOfWork unitOfWork) : IExpansionService
{
    public async Task<IEnumerable<Expansion>> GetAllAsync()
    {
        return await unitOfWork.Expansions.GetAsync();
    }
}
