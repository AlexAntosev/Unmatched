namespace Unmatched.CatalogService.Domain.Services;

using Unmatched.CatalogService.Domain.Entities;

public interface IPlayStyleService
{
    Task<PlayStyle?> AddOrUpdateAsync(PlayStyle playStyle);

    Task<IEnumerable<PlayStyle>> GetAllAsync();

    Task<PlayStyle?> GetAsync(Guid heroId);
}
