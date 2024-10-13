namespace Unmatched.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class PlayStyleRepository : IPlayStyleRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public PlayStyleRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<PlayStyle> AddAsync(PlayStyle model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(PlayStyle model, Guid id)
    {
        throw new NotImplementedException();
    }

    public void AddOrUpdate(PlayStyle model)
    {
        var existingEntity = _dbContext.PlayStyles.FirstOrDefault( x=> x.Id == model.Id);
        if (existingEntity == null)
        {
            _dbContext.Add(model);
        }
        else
        {
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(model);
        }
    }

    public async Task AddRangeAsync(IEnumerable<PlayStyle> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.PlayStyles.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.PlayStyles;
        _dbContext.PlayStyles.RemoveRange(entities);
    }

    public async Task<PlayStyle> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.PlayStyles.FirstOrDefaultAsync(p => p.Id == id);

        return entity;
    }

    public IQueryable<PlayStyle> Query()
    {
        return _dbContext.PlayStyles;
    }

    public async Task<List<PlayStyle>> GetAsync()
    {
        return await _dbContext.PlayStyles.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PlayStyle> GetByHeroIdAsync(Guid heroId)
    {
        var entity = await _dbContext.PlayStyles.FirstOrDefaultAsync(p => p.HeroId == heroId);
        
        return entity;
    }
}
