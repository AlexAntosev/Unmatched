namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

public interface ITournamentAwardRepository : IRepository<TournamentAwardEntity>
{
    Task<IReadOnlyList<TournamentAwardEntity>> GetByTournamentAsync(Guid tournamentId);

    /// <summary>Targeted wipe for <see cref="Services.RatingService.RecalculateAsync"/> - unlike
    /// <see cref="IRepository{T}.DeleteAll"/>, only removes rows of the given kind, so it can reset
    /// Bounty's mutable <see cref="TournamentAwardKind.BountyPool"/> rows before a replay without
    /// touching every other tournament's legitimate completion-bonus awards.</summary>
    Task DeleteByAwardKindAsync(TournamentAwardKind awardKind);
}
