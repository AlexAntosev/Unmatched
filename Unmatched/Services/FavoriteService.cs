namespace Unmatched.Services;

using System;

using Unmatched.Data.Repositories;

public class FavoriteService : IFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;

    public FavoriteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task UpdateFavourAsync(Guid favoriteId, int favour)
    {
        var favorite = _unitOfWork.Favorites.Query().FirstOrDefault(m => m.Id == favoriteId);
        if (favorite is not null)
        {
            favorite.Favour = favour;
            _unitOfWork.Favorites.AddOrUpdate(favorite);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task UpdateChosenOneAsync(Guid favoriteId, bool isChosenOne)
    {
        var chosenOne = _unitOfWork.Favorites.Query().FirstOrDefault(m => m.IsChosenOne);
        if (chosenOne is not null)
        {
            chosenOne.IsChosenOne = false;
            _unitOfWork.Favorites.AddOrUpdate(chosenOne);
            await _unitOfWork.SaveChangesAsync();
        }
        
        var favorite = _unitOfWork.Favorites.Query().FirstOrDefault(m => m.Id == favoriteId);
        if (favorite is not null)
        {
            favorite.IsChosenOne = isChosenOne;
            _unitOfWork.Favorites.AddOrUpdate(favorite);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
