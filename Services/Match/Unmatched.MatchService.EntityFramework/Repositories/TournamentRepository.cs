namespace Unmatched.MatchService.EntityFramework.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.EntityFramework.Context;

public class TournamentRepository(UnmatchedDbContext dbContext) : BaseRepository<Tournament, UnmatchedDbContext>(dbContext), ITournamentRepository
{
    public Guid GetIdByName(string name)
    {
        return DbContext.Tournaments.First(x => x.Name.Equals(name)).Id;
    }

    protected override Guid GetId(Tournament model)
    {
        return model.Id;
    }

    public override async Task<IReadOnlyList<Tournament>> GetAsync()
    {
        return await DbContext.Set<Tournament>().AsNoTracking().ToListAsync();
    }
}
