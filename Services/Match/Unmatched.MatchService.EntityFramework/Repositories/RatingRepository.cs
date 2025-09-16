namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class RatingRepository(UnmatchedDbContext dbContext) : BaseRepository<RatingEntity, UnmatchedDbContext>(dbContext), IRatingRepository
{
    public Task ClearAllRatingsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<RatingEntity?> GetByHeroIdAsync(Guid heroId)
    {
        var entity = await DbContext.Ratings.FirstOrDefaultAsync(r => r.HeroId == heroId);

        return entity;
    }

    protected override Guid GetId(RatingEntity model)
    {
        return model.Id;
    }
}
