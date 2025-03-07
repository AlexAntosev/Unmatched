﻿namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public override Task<List<Tournament>> GetAsync()
    {
        return DbContext.Set<Tournament>().AsNoTracking().ToListAsync();
    }
}
