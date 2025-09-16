namespace Unmatched.MatchService.Domain.Repositories.InMemory;

using Unmatched.MatchService.Domain.Extensions;

public abstract class BaseInMemoryRepository<TEntity> : IRepository<TEntity>
{
    private readonly List<TEntity> _cache = new();
    public async Task<TEntity> AddAsync(TEntity model)
    {
        _cache.Add(model);
        var addedEntity = await GetByIdAsync(GetId(model));
        return addedEntity;
    }

    public void AddOrUpdate(TEntity model, Guid id)
    {
        var existingEntity = GetById(id);
        if (existingEntity == null)
        {
            _cache.Add(model);
        }
        else
        {
            existingEntity = model;
        }
    }

    protected abstract Guid GetId(TEntity model);

    public void AddOrUpdate(TEntity model)
    {
        AddOrUpdate(model, GetId(model));
    }

    public Task AddOrUpdateAsync(TEntity model)
    {
        AddOrUpdate(model);
        return Task.CompletedTask; // redo
    }

    public Task AddRangeAsync(IEnumerable<TEntity> models)
    {
        _cache.AddRange(models);
        return Task.CompletedTask;
    }

    public async Task Delete(Guid id)
    {
        var itemToDelete = await GetByIdAsync(id);
        if (itemToDelete != null)
        {
            _cache.Remove(itemToDelete);
        }
    }

    public void DeleteAll()
    {
        _cache.Clear();
    }

    public IReadOnlyList<TEntity> Get()
    {
        return _cache.Clone();
    }

    public async Task<IReadOnlyList<TEntity>> GetAsync()
    {
        return await Task.FromResult(_cache.Clone());
    }

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(GetById(id));
    }

    protected TEntity? GetById(Guid id)
    {
        return _cache.Find(x => GetId(x) == id);
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}
