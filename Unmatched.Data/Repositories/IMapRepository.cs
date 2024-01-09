namespace Unmatched.Data.Repositories;

using Unmatched.Data.Entities;

public interface IMapRepository : IRepository<Map>
{
    Guid GetIdByName(string name);
}
