using Unmatched.Entities;

namespace Unmatched.Repositories;

public interface IRatingRepository : IRepository<Rating>
{
    Task<Rating> GetByHeroIdAsync(Guid heroId, Guid tournamentId);
}