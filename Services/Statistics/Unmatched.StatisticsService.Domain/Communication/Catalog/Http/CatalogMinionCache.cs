namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

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
