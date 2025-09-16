namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

public interface ICatalogSidekickCache : ICachedService<CatalogSidekickDto>
{
    Task<IEnumerable<CatalogSidekickDto>> GetByHeroAsync(Guid heroId);
}
