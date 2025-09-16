namespace Unmatched.StatisticsService.Domain.Communication.Catalog.Http;

using Unmatched.StatisticsService.Domain.Communication.Catalog.Http.Dto;

public interface ICatalogClient
{
    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();

    Task<IEnumerable<CatalogMapDto>> GetMapsAsync();

    Task<IEnumerable<CatalogSidekickDto>> GetSidekicksAsync();
}
