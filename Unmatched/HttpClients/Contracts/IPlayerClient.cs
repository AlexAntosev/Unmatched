namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos.Player;

public interface IPlayerClient
{
    Task<IEnumerable<PlayerDto>> GetAllAsync();
    Task<Guid?> GetFavouriteHeroIdAsync(Guid playerId);

    Task<Guid> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne);

    Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour);
}