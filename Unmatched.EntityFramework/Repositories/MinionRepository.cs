namespace Unmatched.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class MinionRepository : IMinionRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public MinionRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Minion> AddAsync(Minion model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Minion model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Minion> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Minions.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Minions;
        _dbContext.Minions.RemoveRange(entities);
    }

    public async Task<Minion> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Minions.FindAsync(id);

        return entity;
    }

    public IQueryable<Minion> Query()
    {
        return _dbContext.Minions;
    }

    public async Task<List<Minion>> GetAsync()
    {
        return await _dbContext.Minions.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
