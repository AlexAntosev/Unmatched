namespace Unmatched.MatchService.Domain.Communication.Catalog;

using Unmatched.MatchService.Domain.Cache;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;

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