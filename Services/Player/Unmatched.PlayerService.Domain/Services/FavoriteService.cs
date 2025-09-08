namespace Unmatched.PlayerService.Domain.Services;

using Unmatched.PlayerService.Domain.Repositories;

public class FavoriteService(IUnitOfWork unitOfWork) : IFavoriteService
{
    public async Task<Guid?> GetFavouriteHeroAsync(Guid playerId)
    {
        var favourites = (await unitOfWork.Favorites.GetByPlayerIdAsync(playerId)).OrderByDescending(x => x.IsChosenOne).ThenByDescending(x => x.Favour).ToArray();

        var favouriteHeroId = favourites.FirstOrDefault()?.HeroId;

        return favouriteHeroId;
    }

    public async Task UpdateChosenOneAsync(Guid playerId, Guid favoriteId, bool isChosenOne)
    {
        var chosenOne = unitOfWork.Favorites.Query().FirstOrDefault(m => m.IsChosenOne && m.PlayerId == playerId);
        if (chosenOne is not null)
        {
            chosenOne.IsChosenOne = false;
            unitOfWork.Favorites.AddOrUpdate(chosenOne);
            await unitOfWork.SaveChangesAsync();
        }

        var favorite = unitOfWork.Favorites.Query().FirstOrDefault(m => m.Id == favoriteId);
        if (favorite is not null)
        {
            favorite.IsChosenOne = isChosenOne;
            unitOfWork.Favorites.AddOrUpdate(favorite);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task UpdateFavourAsync(Guid favoriteId, int favour)
    {
        var favorite = unitOfWork.Favorites.Query().FirstOrDefault(m => m.Id == favoriteId);
        if (favorite is not null)
        {
            favorite.Favour = favour;
            unitOfWork.Favorites.AddOrUpdate(favorite);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
