namespace Unmatched.CatalogService.Domain.Repositories;

using Unmatched.CatalogService.Domain.Entities;

public interface IHeroRepository : IRepository<Hero>
{
    Guid GetIdByName(string name);
}
