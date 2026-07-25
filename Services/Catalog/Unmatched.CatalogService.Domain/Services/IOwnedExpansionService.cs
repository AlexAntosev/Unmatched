namespace Unmatched.CatalogService.Domain.Services;

public interface IOwnedExpansionService
{
    Task<IEnumerable<Guid>> GetOwnedExpansionIdsAsync();

    Task ReplaceAsync(IEnumerable<Guid> expansionIds);
}
