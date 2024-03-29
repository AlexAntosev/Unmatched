﻿namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class MapRepository : IMapRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public MapRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Map> AddAsync(Map model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Map model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Map> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Maps.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Maps;
        _dbContext.Maps.RemoveRange(entities);
    }

    public async Task<Map> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Maps.FindAsync(id);

        return entity;
    }

    public IQueryable<Map> Query()
    {
        return _dbContext.Maps;
    }

    public async Task<List<Map>> GetAsync()
    {
        return await _dbContext.Maps.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Guid GetIdByName(string name)
    {
        return _dbContext.Maps.First(x => x.Name.Equals(name)).Id;
    }
}
