namespace Unmatched.Services.Contracts;

using System;

using Unmatched.Dtos;

public interface IFavoriteService
{
    // Task UpdateFavourAsync(Guid favoriteId, int favour);
    //
    // Task UpdateChosenOneAsync(Guid playerId, Guid favoriteId, bool isChosenOne);

    Task<UiHeroDto?> GetFavouriteHeroAsync(Guid playerId);
}