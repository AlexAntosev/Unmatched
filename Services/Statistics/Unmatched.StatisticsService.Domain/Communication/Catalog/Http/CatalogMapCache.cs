namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

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