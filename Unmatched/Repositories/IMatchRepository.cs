namespace Unmatched.Repositories;

using Unmatched.Entities;

public interface IMatchRepository : IRepository<Match>
{
    Match Update(Match model);
}
