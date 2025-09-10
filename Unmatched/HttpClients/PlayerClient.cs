using Unmatched.HttpClients.Contracts;

namespace Unmatched.HttpClients;

using Unmatched.Dtos.Player;

public class PlayerClient : IPlayerClient
{
    public Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Guid?> GetFavouriteHeroIdAsync(Guid playerId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateChosenOneAsync(Guid playerId, Guid heroId, bool isChosenOne)
    {
        throw new NotImplementedException();
    }

    public Task UpdateFavourAsync(Guid playerId, Guid heroId, int favour)
    {
        throw new NotImplementedException();
    }
}
