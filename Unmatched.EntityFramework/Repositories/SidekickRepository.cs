namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class SidekickRepository : ISidekickRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public SidekickRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Sidekick> AddAsync(Sidekick model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Sidekick model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Sidekick> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Sidekicks.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Sidekicks;
        _dbContext.Sidekicks.RemoveRange(entities);
    }

    public async Task<Sidekick> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Sidekicks.FindAsync(id);

        return entity;
    }

    public IQueryable<Sidekick> Query()
    {
        return _dbContext.Sidekicks;
    }

    public Task<List<Sidekick>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
