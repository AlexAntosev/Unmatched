namespace Unmatched.Data.Repositories;

using Unmatched.Common.EntityFramework;
using Unmatched.Data.Entities;

public interface IFavoritesRepository : IRepository<Favorite>
{
    Task<List<Favorite>> GetByPlayerIdAsync(Guid playerId);
}
