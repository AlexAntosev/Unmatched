namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITournamentRepository : IRepository<TournamentEntity>
{
    /// <summary>Loads a tournament with its participants and title selections eager-loaded - the
    /// default <see cref="IRepository{TEntity}.GetByIdAsync"/> doesn't include navigations.</summary>
    Task<TournamentEntity?> GetByIdWithParticipantsAsync(Guid id);
}
