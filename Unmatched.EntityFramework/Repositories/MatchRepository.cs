namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;
using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;
using Unmatched.Data.Enums;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class MatchRepository(UnmatchedDbContext dbContext) : BaseRepository<Match, UnmatchedDbContext>(dbContext), IMatchRepository
{
    public Match Update(Match model)
    {
        var local = DbContext.Set<Match>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(model.Id));

        if (local != null)
        {
            DbContext.Entry(local).State = EntityState.Detached;
        }
        
        DbContext.Update(model);
        return model;
    }

    public async Task<List<Match>> GetFinishedAsync()
    {
        return await DbContext.Matches.Include(x => x.Map).Include(x => x.Tournament).Include(x => x.Fighters).Where(m => !m.IsPlanned).AsNoTracking().ToListAsync();
    }

    public async Task<List<Match>> GetFinishedByHeroIdAsync(Guid heroId)
    {
        return await DbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .Where(m => m.Fighters.Any(f => f.HeroId == heroId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Match>> GetFinishedByMapIdAsync(Guid mapId)
    {
        return await DbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .Where(m => m.MapId.Equals(mapId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Match>> GetFinishedByPlayerIdAsync(Guid playerId)
    {
        return await DbContext.Matches
            .Include(x => x.Fighters)
            .Include(x => x.Map)
            .Include(x => x.Tournament)
            .Where(m => m.Fighters.Any(f => f.PlayerId == playerId) && !m.IsPlanned)
            .OrderByDescending(m => m.Date)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Match>> GetByTournamentAsync(Guid id)
    {
        // return await DbContext.Matches.Include(x => x.Map)
        //     .Include(x => x.Tournament)
        //     .Include(m => m.Fighters)
        //     .ThenInclude(f => f.Hero)
        //     .Include(m => m.Fighters)
        //     .ThenInclude(f => f.Player)
        //     .Where(m => m.TournamentId == id)
        //     .AsNoTracking()
        //     .ToListAsync();
        return await DbContext.Matches.Include(x => x.Map)
            .Include(x => x.Tournament)
            .Include(m => m.Fighters)
            //.ThenInclude(f => f.Hero)
            .Include(m => m.Fighters)
            .ThenInclude(f => f.Player)
            .Where(m => m.TournamentId == id)
            .AsNoTracking()
            .ToListAsync();
    }

    protected override Guid GetId(Match model)
    {
        return model.Id;
    }

    public override Task<List<Match>> GetAsync()
    {
        return DbContext.Set<Match>().Include(x => x.Fighters).AsNoTracking().ToListAsync();
        //return DbContext.Set<Match>().Include(x => x.Fighters).ThenInclude(x => x.Hero).AsNoTracking().ToListAsync();
    }
    public override async Task<Match?> GetByIdAsync(Guid id)
    {
        //var entity = await DbContext.Set<Match>().Include(x => x.Map).Include(x => x.Fighters).ThenInclude(x => x.Hero).ThenInclude(x => x.Sidekicks).Include(x => x.Fighters).ThenInclude(x => x.Player).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        var entity = await DbContext.Set<Match>().Include(x => x.Map).Include(x => x.Fighters).Include(x => x.Fighters).ThenInclude(x => x.Player).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity;
    }
}
