namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface ITournamentRepository : IRepository<Tournament>
{
    Guid GetIdByName(string name);
}
