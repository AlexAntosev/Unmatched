namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITournamentRepository : IRepository<TournamentEntity>
{
    /// <summary>Loads a tournament with its participants and title selections eager-loaded - the
    /// default <see cref="IRepository{TEntity}.GetByIdAsync"/> doesn't include navigations.</summary>
    Task<TournamentEntity?> GetByIdWithParticipantsAsync(Guid id);

    /// <summary>Every completed tournament with its participants eager-loaded - used at startup to find
    /// ones whose stored awards no longer match their participants (e.g. after a participant-correcting
    /// migration, or a database restore that reverted an earlier award recomputation).</summary>
    Task<IReadOnlyList<TournamentEntity>> GetCompletedWithParticipantsAsync();
}
