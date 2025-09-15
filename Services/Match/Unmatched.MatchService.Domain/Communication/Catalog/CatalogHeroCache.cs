namespace Unmatched.MatchService.Domain.Communication.Catalog;

using Unmatched.MatchService.Domain.Cache;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;

public class CatalogHeroCache(ICatalogClient catalogClient) : InMemoryCachedService<CatalogHeroDto>, ICatalogHeroCache
{
    protected override Guid GetId(CatalogHeroDto entity)
    {
        return entity.Id;
    }

    protected override Task<IEnumerable<CatalogHeroDto>> LoadCacheAsync()
    {
        return catalogClient.GetHeroesAsync();
    }
}
