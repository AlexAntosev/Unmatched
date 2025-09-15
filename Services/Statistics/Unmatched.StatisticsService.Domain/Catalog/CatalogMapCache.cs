namespace Unmatched.StatisticsService.Domain.Catalog;

using Unmatched.StatisticsService.Domain.Catalog.Dto;

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