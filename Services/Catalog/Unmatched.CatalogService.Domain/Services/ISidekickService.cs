namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;

public interface ISidekickService
{
    Task<IEnumerable<Sidekick>> GetAsync();

    Task<IEnumerable<Sidekick>> GetByHeroAsync(Guid heroId);
}