namespace Unmatched.MatchService.Domain.Catalog;

using Unmatched.MatchService.Domain.Dto.Catalog;

public interface ICatalogClient
{
    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();
}
