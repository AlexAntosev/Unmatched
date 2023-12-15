namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface IMatchStageRepository : IRepository<MatchStage>
{
    Task<MatchStage> GetByMatchIdAsync(Guid matchId);
}
