namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Dto.Catalog;

public interface ICatalogMapCache : IInMemoryCachedService
{
    Task<IEnumerable<CatalogMapDto>> GetAsync();
}