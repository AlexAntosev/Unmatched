using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class OpponentRepository : IOpponentRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public OpponentRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Opponent> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Opponents.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<Opponent> Query()
    {
        return _dbContext.Opponents;
    }

    public async Task<Opponent> AddAsync(Opponent model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Opponents.FindAsync(id);
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