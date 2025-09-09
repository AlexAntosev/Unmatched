namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos;
using Unmatched.Dtos.Catalog;

public interface ICatalogClient
{
    Task<CatalogHeroDto> GetHeroAsync(Guid id);

    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();

    Task<IEnumerable<CatalogMapDto>> GetMapsAsync();

    Task<Guid> UpdatePlayStyleAsync(PlayStyleDto playStyle);
}