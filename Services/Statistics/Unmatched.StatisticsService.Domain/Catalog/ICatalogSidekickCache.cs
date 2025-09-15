namespace Unmatched.StatisticsService.Domain.Catalog;

using Unmatched.StatisticsService.Domain.Catalog.Dto;

public interface ICatalogSidekickCache : ICachedService<CatalogSidekickDto>
{
    Task<IEnumerable<CatalogSidekickDto>> GetByHeroAsync(Guid heroId);
}
