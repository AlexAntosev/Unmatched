namespace Unmatched.MatchService.Domain.Communication.Catalog;

using Unmatched.MatchService.Domain.Cache;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;

public class CatalogVillainCache(ICatalogClient catalogClient) : InMemoryCachedService<CatalogVillainDto>, ICatalogVillainCache
{
    protected override Guid GetId(CatalogVillainDto entity)
    {
        return entity.Id;
    }

    protected override Task<IEnumerable<CatalogVillainDto>> LoadCacheAsync()
    {
        return catalogClient.GetVillainsAsync();
    }
}
