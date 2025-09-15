namespace Unmatched.MatchService.Domain.Cache;

public abstract class InMemoryCachedService<T> : ICachedService<T>
{
    private IEnumerable<T> _cache = new List<T>();

    public async Task<IEnumerable<T>> GetAsync()
    {
        if (_cache.Any() == false)
        {
            _cache = await LoadCacheAsync();
        }

        return _cache;
    }

    public async Task<T?> GetAsync(Guid id)
    {
        var allItems = await GetAsync();
        var result = allItems.FirstOrDefault(x => GetId(x) == id);
        return result;
    }

    protected abstract Guid GetId(T entity);

    protected abstract Task<IEnumerable<T>> LoadCacheAsync();
}
