namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class FighterRepository(UnmatchedDbContext dbContext) : BaseRepository<FighterEntity, UnmatchedDbContext>(dbContext), IFighterRepository
{
    public async Task<List<FighterEntity>> GetByMatchIdAsync(Guid matchId)
    {
        //return await DbContext.Fighters.Include(x => x.Player).Include(x => x.Hero).ThenInclude(h => h.Sidekicks).Where(x => matchId == x.MatchId).ToListAsync();
        return await DbContext.Fighters.Where(x => matchId == x.MatchId).ToListAsync();
    }

    public async Task<List<FighterEntity>> GetFromFinishedMatchesAsync()
    {
        return await DbContext.Fighters.Include(x => x.Match).Where(x => !x.Match.IsPlanned).ToListAsync();
    }

    public async Task<List<FighterEntity>> GetFromFinishedMatchesByHeroAndPlayerIdAsync(Guid heroId, Guid playerId)
    {
        return await DbContext.Fighters.Include(x => x.Match)
            .Where(x => x.HeroId.Equals(heroId) && x.PlayerId.Equals(playerId) && !x.Match.IsPlanned)
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
    }

    public async Task<List<FighterEntity>> GetFromFinishedMatchesByHeroIdAsync(Guid heroId)
    {
        return await DbContext.Fighters.Include(x => x.Match).Where(x => x.HeroId.Equals(heroId) && !x.Match.IsPlanned).OrderByDescending(x => x.Match.Date).ToListAsync();
    }

    public async Task<List<FighterEntity>> GetFromFinishedMatchesByPlayerIdAsync(Guid playerId)
    {
        return await DbContext.Fighters.Include(x => x.Match)
            .Where(x => x.PlayerId.Equals(playerId) && !x.Match.IsPlanned)
            .OrderByDescending(x => x.Match.Date)
            .ToListAsync();
    }

    protected override Guid GetId(FighterEntity model)
    {
        return model.Id;
    }
}
