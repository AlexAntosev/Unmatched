namespace Unmatched.MatchService.Domain.Communication.Catalog;

using Unmatched.MatchService.Domain.Cache;
using Unmatched.MatchService.Domain.Communication.Catalog.Dto;

public interface ICatalogSidekickCache : ICachedService<CatalogSidekickDto>
{
    Task<IEnumerable<CatalogSidekickDto>> GetByHeroAsync(Guid heroId);
}
