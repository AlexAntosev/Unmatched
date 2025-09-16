namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

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
