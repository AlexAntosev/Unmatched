namespace Unmatched.CatalogService.Domain.Repositories;

using Unmatched.CatalogService.Domain.Entities;

public interface IMapRepository : IRepository<Map>
{
    Guid GetIdByName(string name);
}
