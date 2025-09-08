namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos.Player;

public interface IPlayerClient
{
    Task<IEnumerable<PlayerDto>> GetAllAsync();
}