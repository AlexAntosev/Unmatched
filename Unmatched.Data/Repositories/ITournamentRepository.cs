namespace Unmatched.Data.Repositories;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;

public interface ITournamentRepository : IRepository<Tournament>
{
    Guid GetIdByName(string name);
}
