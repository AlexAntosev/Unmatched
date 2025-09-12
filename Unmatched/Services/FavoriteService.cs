namespace Unmatched.Services;

using AutoMapper;

using Unmatched.Dtos;
using Unmatched.HttpClients.Contracts;
using Unmatched.Services.Contracts;

public class FavoriteService(IMapper mapper, IPlayerClient playerClient, ICatalogClient catalogClient) : IFavoriteService
{
    public async Task<UiHeroDto?> GetFavouriteHeroAsync(Guid playerId)
    {
        var heroId = await playerClient.GetFavouriteHeroIdAsync(playerId);

        var catalogHero = heroId != null
            ? await catalogClient.GetHeroAsync(heroId.Value)
            : null;

        var sidekicks = heroId != null
            ? await catalogClient.GetHeroAsync(heroId.Value)
            : null;

        var hero = mapper.Map<UiHeroDto>(catalogHero);
        // fill heroDto with other stuff

        return hero;
    }

    public Task<Guid> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne)
    {
        return playerClient.UpdateChosenOneAsync(playerId, heroId, isChosenOne);
    }

    public Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour)
    {
        return playerClient.UpdateFavourAsync(playerId, heroId, favour);
    }
}
