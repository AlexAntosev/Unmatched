namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Models.Catalog;

public interface ICatalogClient
{
    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();

    Task<IEnumerable<CatalogMapDto>> GetMapsAsync();

    Task<IEnumerable<CatalogSidekickDto>> GetSidekicksAsync();
}
