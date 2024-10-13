namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class RatingRepository : IRatingRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public RatingRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Rating> AddAsync(Rating model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Rating model, Guid id)
    {
        throw new NotImplementedException();
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

    public void DeleteAll()
    {
        var entities = _dbContext.Ratings;
        _dbContext.Ratings.RemoveRange(entities);
    }

    public async Task<Rating> GetByHeroIdAsync(Guid heroId)
    {
        var entity = await _dbContext.Ratings.FirstOrDefaultAsync(r => r.HeroId == heroId);

        return entity;
    }

    public Task ClearAllRatingsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Rating> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Ratings.FindAsync(id);

        return entity;
    }

    public IQueryable<Rating> Query()
    {
        return _dbContext.Ratings;
    }

    public async Task<List<Rating>> GetAsync()
    {
        return await _dbContext.Ratings.Include(r => r.Hero).OrderByDescending(r => r.Points).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
