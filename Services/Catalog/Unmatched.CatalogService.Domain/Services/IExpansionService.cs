namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;

public interface IExpansionService
{
    Task<IEnumerable<Expansion>> GetAllAsync();
}
