namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

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
