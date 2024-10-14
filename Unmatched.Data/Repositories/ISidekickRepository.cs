namespace Unmatched.Data.Repositories;

using Unmatched.Data.Entities;

public interface ISidekickRepository : IRepository<Sidekick>
{
    IEnumerable<Sidekick> GetByHero(Guid heroId);
}
