﻿using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class HeroRepository : IHeroRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public HeroRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Hero> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Heroes.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<Hero> Query()
    {
        return _dbContext.Heroes;
    }

    public async Task<Hero> AddAsync(Hero model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
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

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}