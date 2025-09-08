namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Dto.Catalog;

public class CatalogHeroCache(ICatalogClient catalogClient) : InMemoryCachedService<CatalogHeroDto>, ICatalogHeroCache
{
    public Task<IEnumerable<CatalogHeroDto>> GetAsync()
    {
        return catalogClient.GetHeroesAsync();
    }

    public async Task<CatalogHeroDto> GetAsync(Guid id)
    {
        var all =  await catalogClient.GetHeroesAsync();
        var hero = all.FirstOrDefault(x => x.Id == id);
        return hero;
    }
}