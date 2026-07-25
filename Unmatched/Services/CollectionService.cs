namespace Unmatched.Services;

using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class CollectionService(ICatalogClient catalogClient) : ICollectionService
{
    public Task<IEnumerable<Guid>> GetOwnedExpansionIdsAsync()
    {
        return catalogClient.GetOwnedExpansionIdsAsync();
    }

    public Task SetOwnedExpansionIdsAsync(IEnumerable<Guid> expansionIds)
    {
        return catalogClient.SetOwnedExpansionIdsAsync(expansionIds);
    }
}
