namespace Unmatched.EntityFramework.Repositories;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class TournamentRepository : BaseRepository<Tournament>, ITournamentRepository
{
    public TournamentRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    public Guid GetIdByName(string name)
    {
        return DbContext.Tournaments.First(x => x.Name.Equals(name)).Id;
    }

    protected override Guid GetId(Tournament model)
    {
        return model.Id;
    }
}
