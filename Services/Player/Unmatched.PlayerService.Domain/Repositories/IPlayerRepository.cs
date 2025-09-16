namespace Unmatched.PlayerService.Domain.Repositories;

using Unmatched.PlayerService.Domain.Entities;

public interface IPlayerRepository : IRepository<Player>
{
    Guid GetIdByName(string name);
    
    Task<List<Player>> GetOleksAndAndrewAsync();
}