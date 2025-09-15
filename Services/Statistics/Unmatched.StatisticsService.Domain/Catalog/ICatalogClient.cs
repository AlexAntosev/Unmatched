namespace Unmatched.StatisticsService.Domain.Catalog;

using Unmatched.StatisticsService.Domain.Catalog.Dto;

public interface ICatalogClient
{
    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();

    Task<IEnumerable<CatalogMapDto>> GetMapsAsync();

    Task<IEnumerable<CatalogSidekickDto>> GetSidekicksAsync();
}
