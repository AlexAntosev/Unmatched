namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class MatchRepository(UnmatchedDbContext dbContext) : BaseRepository<MatchEntity, UnmatchedDbContext>(dbContext), IMatchRepository
{
    public MatchEntity Update(MatchEntity model)
    {
        var local = DbContext.Set<MatchEntity>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(model.Id));

        if (local != null)
        {
            DbContext.Entry(local).State = EntityState.Detached;
        }
        
        DbContext.Update(model);
        return model;
    }

    public async Task<List<MatchEntity>> GetFinishedAsync()
    {
        return await DbContext.Matches.Include(x => x.Tournament).Include(x => x.Fighters).Where(m => !m.IsPlanned).AsNoTracking().ToListAsync();
    }

    public async Task<List<MatchEntity>> GetFinishedByHeroIdAsync(Guid heroId)
    {
        return await DbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Tournament)
            .Where(m => m.Fighters.Any(f => f.HeroId == heroId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MatchEntity>> GetFinishedByMapIdAsync(Guid mapId)
    {
        return await DbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Tournament)
            .Where(m => m.MapId.Equals(mapId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MatchEntity>> GetFinishedByPlayerIdAsync(Guid playerId)
    {
        return await DbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Tournament)
            .Where(m => m.Fighters.Any(f => f.PlayerId == playerId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<MatchEntity>> GetByTournamentAsync(Guid id)
    {
        return await DbContext.Matches
            .Include(x => x.Tournament)
            .Include(m => m.Fighters)
            .Where(m => m.TournamentId == id)
            .AsNoTracking()
            .ToListAsync();
    }

    protected override Guid GetId(MatchEntity model)
    {
        return model.Id;
    }

    public override async Task<IReadOnlyList<MatchEntity>> GetAsync()
    {
        return await DbContext.Set<MatchEntity>().Include(x => x.Fighters).AsNoTracking().ToListAsync();
    }
    public override async Task<MatchEntity?> GetByIdAsync(Guid id)
    {
        var entity = await DbContext.Set<MatchEntity>().Include(x => x.Fighters).Include(x => x.Fighters).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity;
    }
}
