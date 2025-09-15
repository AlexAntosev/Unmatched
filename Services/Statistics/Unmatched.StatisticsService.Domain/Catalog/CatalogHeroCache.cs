namespace Unmatched.StatisticsService.Domain.Catalog;

using Unmatched.StatisticsService.Domain.Catalog.Dto;

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
