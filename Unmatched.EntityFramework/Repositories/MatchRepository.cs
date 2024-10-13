namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;
using Unmatched.Data.Entities;
using Unmatched.Data.Enums;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

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

    public void AddOrUpdate(Match model, Guid id)
    {
        throw new NotImplementedException();
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

    public async Task<List<Match>> GetFinishedAsync()
    {
        return await _dbContext.Matches.Include(x => x.Map).Include(x => x.Tournament).Where(m => !m.IsPlanned).ToListAsync();
    }

    public async Task<List<Match>> GetFinishedByHeroIdAsync(Guid heroId)
    {
        return await _dbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .Where(m => m.Fighters.Any(f => f.HeroId == heroId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    public async Task<List<Match>> GetFinishedByMapIdAsync(Guid mapId)
    {
        return await _dbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .Where(m => m.MapId.Equals(mapId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    public async Task<List<Match>> GetFinishedByPlayerIdAsync(Guid playerId)
    {
        return await _dbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .Where(m => m.Fighters.Any(f => f.PlayerId == playerId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    public async Task<List<Match>> GetByTournamentAsync(Guid id)
    {
        return await _dbContext.Matches.Include(x => x.Map)
            .Include(x => x.Tournament)
            .Include(m => m.Fighters)
            .ThenInclude(f => f.Hero)
            .Include(m => m.Fighters)
            .ThenInclude(f => f.Player)
            .Where(m => m.TournamentId == id)
            .ToListAsync();
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
            .Include(m => m.Map)
            .FirstOrDefaultAsync(m => m.Id == id);

        return entity;
    }

    public IQueryable<Match> Query()
    {
        return _dbContext.Matches;
    }

    public async Task<List<Match>> GetAsync()
    {
        return await _dbContext.Matches.Include(m => m.Fighters).OrderBy(x => x.Date).AsNoTracking().ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
