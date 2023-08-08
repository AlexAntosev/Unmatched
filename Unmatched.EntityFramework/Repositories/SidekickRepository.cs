using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class SidekickRepository : ISidekickRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public SidekickRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Sidekick> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Sidekicks.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<Sidekick> Query()
    {
        return _dbContext.Sidekicks;
    }

    public async Task<Sidekick> AddAsync(Sidekick model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
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

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}