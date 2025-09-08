using Unmatched.HttpClients.Contracts;

namespace Unmatched.HttpClients;

using Unmatched.Dtos.Player;

public class PlayerClient : IPlayerClient
{
    public Task<IEnumerable<PlayerDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
