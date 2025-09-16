namespace Unmatched.PlayerService.Domain.Services;

using System;

public interface IFavoriteService
{
    Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour);
    
    Task<Guid?> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne);

    Task<Guid?> GetFavouriteHeroIdAsync(Guid playerId);
}
