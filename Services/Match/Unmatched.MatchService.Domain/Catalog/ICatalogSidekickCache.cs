namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Models.Catalog;

public interface ICatalogSidekickCache : ICachedService<CatalogSidekickDto>
{
    Task<IEnumerable<CatalogSidekickDto>> GetByHeroAsync(Guid heroId);
}
