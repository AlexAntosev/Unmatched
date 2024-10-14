namespace Unmatched.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.EntityFramework.Context;

public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
{
    public PlayerRepository(UnmatchedDbContext dbContext)
        : base(dbContext)
    {
    }

    public Guid GetIdByName(string name)
    {
        return DbContext.Players.First(x => x.Name.Equals(name)).Id;
    }

    public async Task<List<Player>> GetOleksAndAndrewAsync()
    {
        return await DbContext.Players.Where(p => p.Name.Equals(PlayerNames.Andrii) || p.Name.Equals(PlayerNames.Oleksandr)).ToListAsync();
    }

    protected override Guid GetId(Player model)
    {
        return model.Id;
    }
}
