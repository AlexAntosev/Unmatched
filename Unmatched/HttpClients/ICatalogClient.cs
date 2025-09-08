namespace Unmatched.HttpClients;

using Unmatched.Dtos;
using Unmatched.Dtos.Catalog;

public interface ICatalogClient
{
    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();
    Task<IEnumerable<CatalogMapDto>> GetMapsAsync();
    Task<Guid> UpdatePlayStyleAsync(PlayStyleDto playStyle);
}
