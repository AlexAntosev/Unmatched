namespace Unmatched.MatchService.Domain.Catalog;

public interface ICachedService<T>
{
    Task<IEnumerable<T>> GetAsync();

    Task<T?> GetAsync(Guid id);
}
