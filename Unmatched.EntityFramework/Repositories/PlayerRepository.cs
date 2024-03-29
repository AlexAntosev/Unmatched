﻿namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class PlayerRepository : IPlayerRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public PlayerRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Player> AddAsync(Player model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Player model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Player> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Players.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Players;
        _dbContext.Players.RemoveRange(entities);
    }

    public async Task<Player> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Players.FindAsync(id);

        return entity;
    }

    public IQueryable<Player> Query()
    {
        return _dbContext.Players;
    }

    public async Task<List<Player>> GetAsync()
    {
        return await _dbContext.Players.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Guid GetIdByName(string name)
    {
        return _dbContext.Players.First(x => x.Name.Equals(name)).Id;
    }

    public async Task<List<Player>> GetOleksAndAndrewAsync()
    {
        return await _dbContext.Players.Where(p => p.Name.Equals(PlayerNames.Andrii) || p.Name.Equals(PlayerNames.Oleksandr)).ToListAsync();
    }
}
