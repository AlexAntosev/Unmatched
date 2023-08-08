using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class DuelMatchRepository : IDuelMatchRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public DuelMatchRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<DuelMatch> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.DuelMatches.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<DuelMatch> Query()
    {
        return _dbContext.DuelMatches;
    }

    public async Task<DuelMatch> AddAsync(DuelMatch model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.DuelMatches.FindAsync(id);
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