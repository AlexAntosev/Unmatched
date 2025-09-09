namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITournamentRepository : IRepository<TournamentEntity>
{
    Guid GetIdByName(string name);
}
