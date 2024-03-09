namespace Unmatched.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class VillainRepository : IVillainRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public VillainRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Villain> AddAsync(Villain model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Villain model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Villain> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Villains.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Villains;
        _dbContext.Villains.RemoveRange(entities);
    }

    public async Task<Villain> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Villains.FindAsync(id);

        return entity;
    }

    public IQueryable<Villain> Query()
    {
        return _dbContext.Villains;
    }

    public async Task<List<Villain>> GetAsync()
    {
        return await _dbContext.Villains.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
