namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Dto.Catalog;

public class CatalogMapCache(ICatalogClient client) : ICatalogMapCache
{
    public Task<IEnumerable<CatalogMapDto>> GetAsync()
    {
        return client.GetMapsAsync();
    }
}
