namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Models.Catalog;

public class CatalogMapCache(ICatalogClient client) : InMemoryCachedService<CatalogMapDto>, ICatalogMapCache
{
    protected override Guid GetId(CatalogMapDto entity)
    {
        return entity.Id;
    }

    protected override Task<IEnumerable<CatalogMapDto>> LoadCacheAsync()
    {
        return client.GetMapsAsync();
    }
}