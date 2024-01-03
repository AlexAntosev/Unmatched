namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Entities;
using Unmatched.EntityFramework.Context;
using Unmatched.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public MatchRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Match> AddAsync(Match model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Match model)
    {
        _dbContext.Update(model);
    }

    public Match Update(Match model)
    {
        var local = _dbContext.Set<Match>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(model.Id));

        if (local != null)
        {
            _dbContext.Entry(local).State = EntityState.Detached;
        }
        
        _dbContext.Update(model);
        return model;
    }

    public async Task AddRangeAsync(IEnumerable<Match> models)
    {
        await _dbContext.AddRangeAsync(models);
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

    public void DeleteAll()
    {
        var entities = _dbContext.Matches;
        _dbContext.Matches.RemoveRange(entities);
    }

    public async Task<Match> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Matches
            .AsNoTracking()
            .Include(m => m.Fighters)
                .ThenInclude(f => f.Hero)
                .ThenInclude(f => f.Sidekicks)
            .Include(m => m.Fighters)
                .ThenInclude(f => f.Player)
            .FirstOrDefaultAsync(m => m.Id == id);

        return entity;
    }

    public IQueryable<Match> Query()
    {
        return _dbContext.Matches;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
