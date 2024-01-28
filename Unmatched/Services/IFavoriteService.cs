namespace Unmatched.Services;

using System;

public interface IFavoriteService
{
    Task UpdateFavourAsync(Guid favoriteId, int favour);
    
    Task UpdateChosenOneAsync(Guid playerId, Guid favoriteId, bool isChosenOne);
}
