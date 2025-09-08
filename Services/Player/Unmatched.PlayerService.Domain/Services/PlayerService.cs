using Unmatched.PlayerService.Domain.Entities;

namespace Unmatched.PlayerService.Domain.Services;

using Unmatched.PlayerService.Domain.Repositories;

public class PlayerService(IUnitOfWork unitOfWork) : IPlayerService
{
    public async Task<IEnumerable<Player>> GetAsync()
    {
        var players = await unitOfWork.Players.GetAsync();
        return players;
    }

    public async Task AddAsync(Player player)
    {
        await unitOfWork.Players.AddAsync(player);
        await unitOfWork.SaveChangesAsync();
    }
}
