namespace Unmatched.Services.Contracts;

using Unmatched.Dtos.Player;

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAsync();
    
    Task AddAsync(PlayerDto dto);
}