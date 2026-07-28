namespace Unmatched.PlayerService.Domain.Services;

using Unmatched.PlayerService.Domain.Entities;
using Unmatched.PlayerService.Domain.Repositories;

public class FavoriteService(IUnitOfWork unitOfWork) : IFavoriteService
{
    public async Task<IEnumerable<Favorite>> GetFavoritesAsync(Guid playerId)
    {
        return await unitOfWork.Favorites.GetByPlayerIdAsync(playerId);
    }

    public async Task<Guid?> GetFavouriteHeroIdAsync(Guid playerId)
    {
        var favourites = (await unitOfWork.Favorites.GetByPlayerIdAsync(playerId)).OrderByDescending(x => x.IsChosenOne).ThenByDescending(x => x.Favour).ToArray();

        var favouriteHeroId = favourites.FirstOrDefault()?.HeroId;

        return favouriteHeroId;
    }

    public async Task<Guid?> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne)
    {
        var chosenOne = unitOfWork.Favorites.Query().FirstOrDefault(m => m.IsChosenOne && m.PlayerId == playerId);
        if (chosenOne is not null && chosenOne.HeroId != heroId)
        {
            chosenOne.IsChosenOne = false;
            unitOfWork.Favorites.AddOrUpdate(chosenOne);
        }

        // A hero can be played (and therefore "made main") long before it ever picks up a Favorite
        // row, since one is only created the first time it is favourited or rated.
        var favorite = GetOrCreateFavorite(playerId, heroId);
        favorite.IsChosenOne = isChosenOne;
        unitOfWork.Favorites.AddOrUpdate(favorite);

        await unitOfWork.SaveChangesAsync();
        return favorite.HeroId;
    }

    public async Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour)
    {
        var favorite = GetOrCreateFavorite(playerId, heroId);
        favorite.Favour = favour;
        unitOfWork.Favorites.AddOrUpdate(favorite);
        await unitOfWork.SaveChangesAsync();
    }

    private Favorite GetOrCreateFavorite(Guid playerId, Guid heroId)
        => unitOfWork.Favorites.Query().FirstOrDefault(m => m.PlayerId == playerId && m.HeroId == heroId)
        ?? new Favorite { Id = Guid.NewGuid(), PlayerId = playerId, HeroId = heroId };
}
