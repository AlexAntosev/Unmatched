namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class HeroTitleRepository(UnmatchedDbContext dbContext) : IHeroTitleRepository
{
    public async Task AddOrUpdateAsync(HeroTitleEntity model)
    {
        var existing = await dbContext.Set<HeroTitleEntity>().FirstOrDefaultAsync(x => x.HeroesId == model.HeroesId && x.TitlesId == model.TitlesId);
        if (existing == null)
        {
            await dbContext.Set<HeroTitleEntity>().AddAsync(model);
        }
        else
        {
            dbContext.Entry(existing).CurrentValues.SetValues(model);
        }
    }
}
