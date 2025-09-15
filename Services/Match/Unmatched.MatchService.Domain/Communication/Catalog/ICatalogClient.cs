namespace Unmatched.MatchService.Domain.Communication.Catalog;

using Unmatched.MatchService.Domain.Communication.Catalog.Dto;

public interface ICatalogClient
{
    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();

    Task<IEnumerable<CatalogMapDto>> GetMapsAsync();

    Task<IEnumerable<CatalogSidekickDto>> GetSidekicksAsync();
}