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

public interface IMatchClient
{
    Task<SaveMatchResultDto> AddAsync(MatchDto match);

    Task<SaveMatchResultDto> UpdateAsync(MatchDto match);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();
}
