namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Repositories;

public abstract class BaseRepository<TEntity, TContext>(TContext dbContext) : IRepository<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    protected TContext DbContext { get; } = dbContext;

    public async Task<TEntity> AddAsync(TEntity model)
    {
        var addedEntity = await DbContext.Set<TEntity>().AddAsync(model);
        return addedEntity.Entity;
    }

    public void AddOrUpdate(TEntity model, Guid id)
    {
        var existingEntity = DbContext.Set<TEntity>().Find(id);
        if (existingEntity == null)
        {
            DbContext.Set<TEntity>().Add(model);
        }
        else
        {
            DbContext.Entry(existingEntity).CurrentValues.SetValues(model);
        }
    }

    public void AddOrUpdate(TEntity model)
    {
        var modelId = GetId(model);
        AddOrUpdate(model, modelId);
    }

    public async Task AddOrUpdateAsync(TEntity model)
    {
        var existing = await GetByIdAsync(GetId(model));
        if (existing == null)
        {
            await AddAsync(model);
        }
        else
        {
            DbContext.Entry(existing).CurrentValues.SetValues(model);
        }
    }

    protected abstract Guid GetId(TEntity model);

    public Task AddRangeAsync(IEnumerable<TEntity> models)
    {
        return DbContext.Set<TEntity>().AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        if (entity is null)
        {
            return;
        }

        DbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = DbContext.Set<TEntity>();
        DbContext.Set<TEntity>().RemoveRange(entities);
    }

    public IReadOnlyList<TEntity> Get()
    {
        return DbContext.Set<TEntity>().AsNoTracking().ToList();
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAsync()
    {
        return await DbContext.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var entity = (await DbContext.Set<TEntity>().AsNoTracking().ToListAsync()).FirstOrDefault(x => GetId(x) == id);
        return entity;
    }

    public Task SaveChangesAsync()
    {
        return DbContext.SaveChangesAsync();
    }
}
