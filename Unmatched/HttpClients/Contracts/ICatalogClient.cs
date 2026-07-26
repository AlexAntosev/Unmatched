namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos.Catalog;

public interface ICatalogClient
{
    Task<CatalogHeroDto> GetHeroAsync(Guid id);

    Task<IEnumerable<CatalogHeroDto>> GetHeroesAsync();

    Task<IEnumerable<CatalogMapDto>> GetMapsAsync();

    Task<IEnumerable<CatalogSidekickDto>> GetSidekicksByHeroAsync(Guid heroId);

    Task<Guid> UpdatePlayStyleAsync(CatalogPlayStyleDto playStyle);

    Task<CatalogPlayStyleDto> GetPlayStyleByHero(Guid heroId);

    Task<IEnumerable<CatalogExpansionDto>> GetExpansionsAsync();

    Task<IEnumerable<Guid>> GetOwnedExpansionIdsAsync();

    Task SetOwnedExpansionIdsAsync(IEnumerable<Guid> expansionIds);

    Task<IEnumerable<CatalogVillainDto>> GetVillainsAsync();

    Task<IEnumerable<CatalogMinionDto>> GetMinionsAsync();

    Task<CatalogHeroDto> UpdateHeroImageAsync(Guid heroId, string imageFileName);

    Task<CatalogVillainDto> UpdateVillainImageAsync(Guid villainId, string imageFileName);

    Task<CatalogMinionDto> UpdateMinionImageAsync(Guid minionId, string imageFileName);
}
