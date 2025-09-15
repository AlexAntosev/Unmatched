namespace Unmatched.StatisticsService.Domain.Catalog;

using Unmatched.StatisticsService.Domain.Catalog.Dto;

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
