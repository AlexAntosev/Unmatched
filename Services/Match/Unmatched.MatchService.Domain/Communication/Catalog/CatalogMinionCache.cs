namespace Unmatched.MatchService.Domain.Communication.Catalog;

using Unmatched.MatchService.Domain.Cache;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;

public class CatalogMinionCache(ICatalogClient catalogClient) : InMemoryCachedService<CatalogMinionDto>, ICatalogMinionCache
{
    protected override Guid GetId(CatalogMinionDto entity)
    {
        return entity.Id;
    }

    protected override Task<IEnumerable<CatalogMinionDto>> LoadCacheAsync()
    {
        return catalogClient.GetMinionsAsync();
    }
}
