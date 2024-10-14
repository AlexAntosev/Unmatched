namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class RatingRepository : BaseRepository<Rating>, IRatingRepository
{
    public RatingRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

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
