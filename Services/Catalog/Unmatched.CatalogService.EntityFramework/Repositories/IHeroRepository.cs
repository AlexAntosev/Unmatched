namespace Unmatched.CatalogService.EntityFramework.Repositories;

using Unmatched.CatalogService.EntityFramework.Entities;
using Unmatched.Common.EntityFramework;

public interface IHeroRepository : IRepository<Hero>
{
    Guid GetIdByName(string name);
}
