namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class MapRepository : IMapRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public MapRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Map> AddAsync(Map model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Map model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Map> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Maps.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public async Task<Map> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Maps.FindAsync(id);

        return entity;
    }

    public IQueryable<Map> Query()
    {
        return _dbContext.Maps;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
