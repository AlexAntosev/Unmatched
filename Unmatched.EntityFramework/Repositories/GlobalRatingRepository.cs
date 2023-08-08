using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class GlobalRatingRepository : IGlobalRatingRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public GlobalRatingRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<GlobalRating> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.GlobalRatings.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<GlobalRating> Query()
    {
        return _dbContext.GlobalRatings;
    }

    public async Task<GlobalRating> AddAsync(GlobalRating model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.GlobalRatings.FindAsync(id);
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