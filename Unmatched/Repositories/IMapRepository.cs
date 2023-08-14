namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface IMapRepository : IRepository<Map>
{
    Guid GetIdByName(string name);
}
