namespace Unmatched.PlayerService.Domain.Services;

using Unmatched.PlayerService.Domain.Repositories;

public class FavoriteService(IUnitOfWork unitOfWork) : IFavoriteService
{
    public async Task<Guid?> GetFavouriteHeroIdAsync(Guid playerId)
    {
        var favourites = (await unitOfWork.Favorites.GetByPlayerIdAsync(playerId)).OrderByDescending(x => x.IsChosenOne).ThenByDescending(x => x.Favour).ToArray();

        var favouriteHeroId = favourites.FirstOrDefault()?.HeroId;

        return favouriteHeroId;
    }

    public async Task<Guid?> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne)
    {

        var chosenOne = unitOfWork.Favorites.Query().FirstOrDefault(m => m.IsChosenOne && m.PlayerId == playerId);
        if (chosenOne is not null)
        {
            chosenOne.IsChosenOne = false;
            unitOfWork.Favorites.AddOrUpdate(chosenOne);
        }

        var favorite = unitOfWork.Favorites.Query().FirstOrDefault(m => m.PlayerId == playerId && m.HeroId == heroId);
        if (favorite is not null)
        {
            favorite.IsChosenOne = isChosenOne;
            unitOfWork.Favorites.AddOrUpdate(favorite);
        }
        await unitOfWork.SaveChangesAsync();
        return favorite?.HeroId;
    }

    public async Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour)
    {
        var favorite = unitOfWork.Favorites.Query().FirstOrDefault(m => m.PlayerId == playerId && m.HeroId == heroId);
        if (favorite is not null)
        {
            favorite.Favour = favour;
            unitOfWork.Favorites.AddOrUpdate(favorite);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
