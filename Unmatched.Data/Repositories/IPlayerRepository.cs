namespace Unmatched.Data.Repositories;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;

public interface IPlayerRepository : IRepository<Player>
{
    Guid GetIdByName(string name);
    
    Task<List<Player>> GetOleksAndAndrewAsync();
}
