namespace Unmatched.PlayerService.EntityFramework.Repositories;

using Microsoft.EntityFrameworkCore;

using Unmatched.PlayerService.Domain.Constants;
using Unmatched.PlayerService.Domain.Entities;
using Unmatched.PlayerService.Domain.Repositories;
using Unmatched.PlayerService.EntityFramework.Context;

public class PlayerRepository(UnmatchedDbContext dbContext) : BaseRepository<Player, UnmatchedDbContext>(dbContext), IPlayerRepository
{
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
