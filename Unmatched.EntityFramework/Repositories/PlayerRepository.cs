using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public PlayerRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Player> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Players.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<Player> Query()
    {
        return _dbContext.Players;
    }

    public async Task<Player> AddAsync(Player model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }
    
    public async Task AddRangeAsync(IEnumerable<Player> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Players.FindAsync(id);
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