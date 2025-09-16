namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

public class CatalogSidekickCache(ICatalogClient client) : InMemoryCachedService<CatalogSidekickDto>, ICatalogSidekickCache
{
    protected override Guid GetId(CatalogSidekickDto entity)
    {
        return entity.Id;
    }

    protected override Task<IEnumerable<CatalogSidekickDto>> LoadCacheAsync()
    {
        return client.GetSidekicksAsync();
    }

    public async Task<IEnumerable<CatalogSidekickDto>> GetByHeroAsync(Guid heroId)
    {
        var allEntities = await GetAsync();
        var result = allEntities.Where(x => x.HeroId == heroId);
        return result;
    }
}
