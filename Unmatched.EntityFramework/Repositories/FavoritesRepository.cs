namespace Unmatched.EntityFramework.Repositories;

using System;
using Microsoft.EntityFrameworkCore;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class FavoritesRepository : IFavoritesRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public FavoritesRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Favorite> AddAsync(Favorite model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Favorite model)
    {
        _dbContext.Update(model);
    }

    public async Task AddRangeAsync(IEnumerable<Favorite> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Favorites.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Favorites;
        _dbContext.Favorites.RemoveRange(entities);
    }

    public async Task<Favorite> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Favorites.Include(f => f.Player).Include(f => f.Hero).FirstOrDefaultAsync(f => f.Id == id);

        return entity;
    }

    public IQueryable<Favorite> Query()
    {
        return _dbContext.Favorites.Include(f => f.Player).Include(f => f.Hero);
    }

    public async Task<List<Favorite>> GetAsync()
    {
        return await _dbContext.Favorites.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Favorite>> GetByPlayerIdAsync(Guid playerId)
    {
        return await _dbContext.Favorites.Include(f => f.Player).Include(f => f.Hero).Where(f => f.PlayerId == playerId).ToListAsync();
    }
}
