using Unmatched.Services.Contracts;

namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;

public class FavoriteService(IMapper mapper, IFavoriteClient favoriteClient, ICatalogClient catalogClient) : IFavoriteService
{
    public async Task<HeroDto?> GetFavouriteHeroAsync(Guid playerId)
    {
        var heroId = await favoriteClient.GetFavouriteHeroIdAsync(playerId);

        var catalogHero = heroId != null
            ? await catalogClient.GetHeroAsync(heroId.Value)
            : null;

        var hero = mapper.Map<HeroDto>(catalogHero);
        // fill heroDto with other stuff

        return hero;
    }
}
