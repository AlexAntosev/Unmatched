using Microsoft.EntityFrameworkCore;
using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public RatingRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Rating> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Ratings.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<Rating> Query()
    {
        return _dbContext.Ratings;
    }

    public async Task<Rating> AddAsync(Rating model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Rating model)
    {
        _dbContext.Update(model);
    }

    public async Task AddRangeAsync(IEnumerable<Rating> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Ratings.FindAsync(id);
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

    public async Task<Rating> GetByHeroIdAsync(Guid heroId)
    {
        var entity = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.HeroId == heroId);
            
        return entity;
    }
}