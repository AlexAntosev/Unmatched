namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface IPlayerRepository : IRepository<Player>
{
    Guid GetIdByName(string name);
}
