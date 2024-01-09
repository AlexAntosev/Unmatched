namespace Unmatched.EntityFramework.Repositories;

using System;
using Microsoft.EntityFrameworkCore;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

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
        _dbContext.Update(model);
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
        var entity = await _dbContext.Titles.Include(t => t.Heroes).FirstOrDefaultAsync(t => t.Id == id);

        return entity;
    }

    public IQueryable<Title> Query()
    {
        return _dbContext.Titles.Include(t => t.Heroes);
    }

    public async Task<List<Title>> GetAsync()
    {
        return await _dbContext.Titles.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Title>> GetByHeroId(Guid heroId)
    {
        var entities = await _dbContext.Titles.Include(t => t.Heroes).Where(t => t.Heroes.Any(h => h.Id == heroId)).ToListAsync();

        return entities;
    }

    public async Task<Title?> GetByNameAsync(string name)
    {
        var entity = await _dbContext.Titles.Include(t => t.Heroes).FirstOrDefaultAsync(t => t.Name.Equals(name));

        return entity;
    }
}
