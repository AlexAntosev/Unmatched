namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITournamentAwardRepository : IRepository<TournamentAwardEntity>
{
    Task<IReadOnlyList<TournamentAwardEntity>> GetByTournamentAsync(Guid tournamentId);
}
