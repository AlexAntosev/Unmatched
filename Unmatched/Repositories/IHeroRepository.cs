namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface IHeroRepository : IRepository<Hero>
{
    Guid GetIdByName(string name);
}
