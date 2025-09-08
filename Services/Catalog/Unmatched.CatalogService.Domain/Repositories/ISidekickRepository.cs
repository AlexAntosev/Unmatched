namespace Unmatched.CatalogService.Domain.Repositories;

using Unmatched.CatalogService.Domain.Entities;

public interface ISidekickRepository : IRepository<Sidekick>
{
    IEnumerable<Sidekick> GetByHero(Guid heroId);
}
