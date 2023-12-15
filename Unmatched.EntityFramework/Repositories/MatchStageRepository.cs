namespace Unmatched.EntityFramework.Repositories;

using System;

using Microsoft.EntityFrameworkCore;

using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class MatchStageRepository : IMatchStageRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public MatchStageRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<MatchStage> AddAsync(MatchStage model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(MatchStage model)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<MatchStage> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.MatchStages.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.MatchStages;
        _dbContext.MatchStages.RemoveRange(entities);
    }

    public async Task<MatchStage> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.MatchStages.FindAsync(id);

        return entity;
    }

    public IQueryable<MatchStage> Query()
    {
        return _dbContext.MatchStages;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<MatchStage> GetByMatchIdAsync(Guid matchId)
    {
        return await _dbContext.MatchStages.FirstAsync(x => x.MatchId.Equals(matchId));
    }
}
