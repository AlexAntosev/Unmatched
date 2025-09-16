namespace Unmatched.StatisticsService.EntityFramework.Repositories;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Unmatched.StatisticsService.Domain.Repositories;

public abstract class BaseRepository<TModel, TEntity, TContext>(TContext dbContext, IMapper mapper) : IRepository<TModel>
    where TEntity : class where TContext : DbContext
{
    protected TContext DbContext { get; } = dbContext;

    public async Task<TModel> AddAsync(TModel model)
    {
        var entity = MapToEntity(model);
        var addedModel = await AddAsyncInternal(entity);
        return addedModel;
    }

    public async Task AddOrUpdateAsync(TModel model)
    {
        var entity = MapToEntity(model);
        var existing = await GetByIdTrackedInternalAsync(GetId(entity));
        if (existing == null)
        {
            await AddAsyncInternal(entity);
        }
        else
        {
            DbContext.Entry(existing).CurrentValues.SetValues(entity);
        }
    }

    public Task AddRangeAsync(IEnumerable<TModel> models)
    {
        var entities = models.Select(MapToEntity);
        return DbContext.Set<TEntity>().AddRangeAsync(entities);
    }

    public void DeleteAll()
    {
        var entities = DbContext.Set<TEntity>();
        DbContext.Set<TEntity>().RemoveRange(entities);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        if (entity is null)
        {
            return;
        }

        DbContext.Remove(entity);
    }

    public IReadOnlyList<TModel> GetAll()
    {
        var entities = DbContext.Set<TEntity>().AsNoTracking().ToList();
        var models = entities.Select(MapToModel).ToList();
        return models;
    }

    public virtual async Task<IReadOnlyList<TModel>> GetAllAsync()
    {
        var entities = await DbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        var models = entities.Select(MapToModel).ToList();
        return models;
    }

    public virtual async Task<TModel?> GetAsync(Guid id)
    {
        var entity = (await DbContext.Set<TEntity>().AsNoTracking().ToListAsync()).FirstOrDefault(x => GetId(x) == id);
        var model = MapToModel(entity);
        return model;
    }

    public Task SaveChangesAsync()
    {
        return DbContext.SaveChangesAsync();
    }

    protected abstract Guid GetId(TEntity model);

    protected virtual TEntity MapToEntity(TModel model)
    {
        return mapper.Map<TEntity>(model);
    }

    protected virtual TModel MapToModel(TEntity entity)
    {
        return mapper.Map<TModel>(entity);
    }

    private async Task<TModel> AddAsyncInternal(TEntity entity)
    {
        var addedEntity = await DbContext.Set<TEntity>().AddAsync(entity);
        var addedModel = MapToModel(addedEntity.Entity);
        return addedModel;
    }

    private async Task<TModel?> GetByIdTrackedInternalAsync(Guid id)
    {
        var entity = await DbContext.Set<TEntity>().FindAsync(id);
        var model = MapToModel(entity);
        return model;
    }
}
