namespace Unmatched.PlayerService.Domain.Services;

using Unmatched.PlayerService.Domain.Entities;

public interface IPlayerService
{
    Task<IEnumerable<Player>> GetAsync();
    
    Task AddAsync(Player dto);
}
