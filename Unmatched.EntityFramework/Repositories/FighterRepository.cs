namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class FighterRepository : IFighterRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public FighterRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fighter> AddAsync(Fighter model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Fighter model)
    {
        _dbContext.Update(model);
    }

    public async Task AddRangeAsync(IEnumerable<Fighter> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Fighters.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public async Task<Fighter> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Fighters.FindAsync(id);

        return entity;
    }

    public IQueryable<Fighter> Query()
    {
        return _dbContext.Fighters;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
