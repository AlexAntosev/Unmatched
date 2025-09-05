namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

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
