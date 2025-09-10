namespace Unmatched.CatalogService.Domain.Repositories;

using Unmatched.CatalogService.Domain.Entities;

public interface ISidekickRepository : IRepository<Sidekick>
{
    Task<IEnumerable<Sidekick>> GetByHeroAsync(Guid heroId);
}
