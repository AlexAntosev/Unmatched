namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.EntityFramework.Entities;
using Unmatched.Common.EntityFramework;

public interface IMapRepository : IRepository<Map>
{
    Guid GetIdByName(string name);
}
