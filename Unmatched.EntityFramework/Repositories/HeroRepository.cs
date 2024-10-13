namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class HeroRepository : IHeroRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public HeroRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Hero> AddAsync(Hero model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Hero model, Guid id)
    {
        throw new NotImplementedException();
    }

    public void AddOrUpdate(Hero model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Hero> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Heroes.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Heroes;
        _dbContext.Heroes.RemoveRange(entities);
    }

    public async Task<Hero> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Heroes.Include(h => h.Sidekicks).Include(h => h.PlayStyle).FirstOrDefaultAsync(h => h.Id == id);

        return entity;
    }

    public IQueryable<Hero> Query()
    {
        return _dbContext.Heroes.Include(e => e.Sidekicks);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Guid GetIdByName(string name)
    {
        return _dbContext.Heroes.First(x => x.Name.Equals(name)).Id;
    }
    
    public async Task<List<Hero>> GetAsync()
    {
        return await _dbContext.Heroes.Include(e => e.Sidekicks).OrderBy(h => h.Name).ToListAsync();
    }
}
