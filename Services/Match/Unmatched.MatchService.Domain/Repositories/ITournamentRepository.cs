namespace Unmatched.MatchService.Domain.Repositories;

using Unmatched.MatchService.Domain.Entities;

public interface ITournamentRepository : IRepository<Tournament>
{
    Guid GetIdByName(string name);
}
