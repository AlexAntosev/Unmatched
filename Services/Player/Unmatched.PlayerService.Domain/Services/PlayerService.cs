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

    public async Task<Player?> UpdateImageAsync(Guid id, string imageFileName)
    {
        var player = await unitOfWork.Players.GetByIdAsync(id);
        if (player is null)
        {
            return null;
        }

        player.ImageFileName = imageFileName;
        unitOfWork.Players.AddOrUpdate(player, id);
        await unitOfWork.SaveChangesAsync();
        return player;
    }
}
