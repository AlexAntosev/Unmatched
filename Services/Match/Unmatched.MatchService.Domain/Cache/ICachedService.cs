namespace Unmatched.MatchService.Domain.Cache;

public interface ICachedService<T>
{
    Task<IEnumerable<T>> GetAsync();

    Task<T?> GetAsync(Guid id);
}
