﻿using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

namespace Unmatched.EntityFramework.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public MatchRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Match> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Matches.FindAsync(id);
            
        return entity;
    }

    public IEnumerable<Match> Query()
    {
        return _dbContext.Matches;
    }

    public async Task<Match> AddAsync(Match model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Matches.FindAsync(id);
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