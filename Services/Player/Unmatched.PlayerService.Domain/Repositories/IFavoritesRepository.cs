namespace Unmatched.PlayerService.Domain.Repositories;

using Unmatched.PlayerService.Domain.Entities;

public interface IFavoritesRepository : IRepository<Favorite>
{
    Task<List<Favorite>> GetByPlayerIdAsync(Guid playerId);
}
