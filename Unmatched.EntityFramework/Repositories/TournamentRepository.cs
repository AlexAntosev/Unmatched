using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public TournamentRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Tournament> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Tournaments.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<Tournament> Query()
    {
        return _dbContext.Tournaments;
    }

    public async Task<Tournament> AddAsync(Tournament model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }
    
    public async Task AddRangeAsync(IEnumerable<Tournament> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Tournaments.FindAsync(id);
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