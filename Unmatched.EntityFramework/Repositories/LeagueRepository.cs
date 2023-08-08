using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class LeagueRepository : ILeagueRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public LeagueRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<League> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Leagues.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<League> Query()
    {
        return _dbContext.Leagues;
    }

    public async Task<League> AddAsync(League model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Leagues.FindAsync(id);
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