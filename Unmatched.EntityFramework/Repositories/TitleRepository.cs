namespace Unmatched.EntityFramework.Repositories;

using System;
using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class TitleRepository : ITitleRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public TitleRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Title> AddAsync(Title model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Title model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Title> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Titles.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Titles;
        _dbContext.Titles.RemoveRange(entities);
    }

    public async Task<Title> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Titles.FindAsync(id);

        return entity;
    }

    public IQueryable<Title> Query()
    {
        return _dbContext.Titles;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
