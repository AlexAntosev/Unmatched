using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class MatchParticipantRepository : IMatchParticipantRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public MatchParticipantRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<MatchParticipant> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.MatchParticipants.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<MatchParticipant> Query()
    {
        return _dbContext.MatchParticipants;
    }

    public async Task<MatchParticipant> AddAsync(MatchParticipant model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.MatchParticipants.FindAsync(id);
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