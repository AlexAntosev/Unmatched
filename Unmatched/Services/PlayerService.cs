using Unmatched.Services.Contracts;

namespace Unmatched.Services;

using Unmatched.Dtos.Player;
using Unmatched.HttpClients.Contracts;

public class PlayerService(IPlayerClient playerClient) : IPlayerService
{
    public async Task<IEnumerable<PlayerDto>> GetAsync()
    {
        var players = await playerClient.GetAllAsync();
        return players;
    }

    public Task AddAsync(PlayerDto dto)
    {
        throw new NotImplementedException();
    }
}
