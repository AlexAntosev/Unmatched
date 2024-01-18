namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class FighterRepository : IFighterRepository
{
    private readonly UnmatchedDbContext _dbContext;

    public FighterRepository(UnmatchedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fighter> AddAsync(Fighter model)
    {
        var createdEntityEntry = await _dbContext.AddAsync(model);
        var createdEntity = createdEntityEntry.Entity;

        return createdEntity;
    }

    public void AddOrUpdate(Fighter model)
    {
        _dbContext.Update(model);
    }

    public async Task AddRangeAsync(IEnumerable<Fighter> models)
    {
        await _dbContext.AddRangeAsync(models);
    }

    public async Task Delete(Guid id)
    {
        var entity = await _dbContext.Fighters.FindAsync(id);
        if (entity is null)
        {
            return;
        }

        _dbContext.Remove(entity);
    }

    public void DeleteAll()
    {
        var entities = _dbContext.Fighters;
        _dbContext.Fighters.RemoveRange(entities);
    }

    public async Task<Fighter> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Fighters.FindAsync(id);

        return entity;
    }

    public IQueryable<Fighter> Query()
    {
        return _dbContext.Fighters;
    }
    
    public async Task<List<Fighter>> GetAsync()
    {
        return await _dbContext.Fighters.Include(f => f.Match).OrderBy(f => f.Match.Date).ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Fighter>> GetByMatchIdAsync(Guid matchId)
    {
        return await _dbContext.Fighters
            .Include(x => x.Player)
            .Include(x => x.Hero)
            .ThenInclude(h => h.Sidekicks)
            .Where(x => matchId == x.MatchId)
            .ToListAsync();
    }

    public async Task<List<Fighter>> GetFromFinishedMatchesAsync()
    {
        return await _dbContext.Fighters.Include(x => x.Match).Where(x => !x.Match.IsPlanned).ToListAsync();
    }

    public async Task<List<Fighter>> GetFromFinishedMatchesByHeroIdAsync(Guid heroId)
    {
        return await _dbContext.Fighters
            .Include(x => x.Match)
            .Where(x => x.HeroId.Equals(heroId) && !x.Match.IsPlanned)
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
    }

    public async Task<List<Fighter>> GetFromFinishedMatchesByPlayerIdAsync(Guid playerId)
    {
        return await _dbContext.Fighters
            .Include(x => x.Match)
            .Include(x => x.Player)
            .Where(x => x.PlayerId.Equals(playerId) && !x.Match.IsPlanned)
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
    }
}
