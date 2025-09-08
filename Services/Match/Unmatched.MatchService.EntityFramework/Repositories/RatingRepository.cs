namespace Unmatched.MatchService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class RatingRepository(UnmatchedDbContext dbContext) : BaseRepository<Rating, UnmatchedDbContext>(dbContext), IRatingRepository
{
    public Task ClearAllRatingsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Rating?> GetByHeroIdAsync(Guid heroId)
    {
        var entity = await DbContext.Ratings.FirstOrDefaultAsync(r => r.HeroId == heroId);

        return entity;
    }

    protected override Guid GetId(Rating model)
    {
        return model.Id;
    }
}
