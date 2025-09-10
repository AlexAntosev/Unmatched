namespace Unmatched.Services.Contracts;

using System;

using Unmatched.Dtos;

public interface IFavoriteService
{
    Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour);
    
    Task<Guid> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne);

    Task<UiHeroDto?> GetFavouriteHeroAsync(Guid playerId);
}