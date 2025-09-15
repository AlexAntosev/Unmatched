namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Models.Catalog;

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
