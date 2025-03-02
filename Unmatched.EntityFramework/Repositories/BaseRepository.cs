namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    public BaseRepository(UnmatchedDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected UnmatchedDbContext DbContext { get; }

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

    public virtual Task<List<TEntity>> GetAsync()
    {
        return DbContext.Set<TEntity>().ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        return entity;
    }

    public virtual IQueryable<TEntity> Query(bool noTrack = false)
    {
        return noTrack ? DbContext.Set<TEntity>().AsNoTracking() : DbContext.Set<TEntity>();
    }

    public Task SaveChangesAsync()
    {
        return DbContext.SaveChangesAsync();
    }
}
