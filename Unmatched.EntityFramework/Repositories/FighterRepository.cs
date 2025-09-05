namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class FighterRepository(UnmatchedDbContext dbContext) : BaseRepository<Fighter, UnmatchedDbContext>(dbContext), IFighterRepository
{
    public async Task<List<Fighter>> GetByMatchIdAsync(Guid matchId)
    {
        //return await DbContext.Fighters.Include(x => x.Player).Include(x => x.Hero).ThenInclude(h => h.Sidekicks).Where(x => matchId == x.MatchId).ToListAsync();
        return await DbContext.Fighters.Include(x => x.Player).Where(x => matchId == x.MatchId).ToListAsync();
    }

    public async Task<List<Fighter>> GetFromFinishedMatchesAsync()
    {
        return await DbContext.Fighters.Include(x => x.Match).Where(x => !x.Match.IsPlanned).ToListAsync();
    }

    public async Task<List<Fighter>> GetFromFinishedMatchesByHeroAndPlayerIdAsync(Guid heroId, Guid playerId)
    {
        return await DbContext.Fighters.Include(x => x.Match)
            .Include(x => x.Player)
            .Where(x => x.HeroId.Equals(heroId) && x.PlayerId.Equals(playerId) && !x.Match.IsPlanned)
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
    }

    public async Task<List<Fighter>> GetFromFinishedMatchesByHeroIdAsync(Guid heroId)
    {
        return await DbContext.Fighters.Include(x => x.Match).Where(x => x.HeroId.Equals(heroId) && !x.Match.IsPlanned).OrderByDescending(x => x.Match.Date).ToListAsync();
    }

    public async Task<List<Fighter>> GetFromFinishedMatchesByPlayerIdAsync(Guid playerId)
    {
        return await DbContext.Fighters.Include(x => x.Match)
            .Include(x => x.Player)
            .Where(x => x.PlayerId.Equals(playerId) && !x.Match.IsPlanned)
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
    }

    protected override Guid GetId(Fighter model)
    {
        return model.Id;
    }
}
