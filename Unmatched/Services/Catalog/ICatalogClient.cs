namespace Unmatched.Services.Catalog;

using Unmatched.Dtos.Catalog;

public interface ICatalogClient
{
    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();
}
