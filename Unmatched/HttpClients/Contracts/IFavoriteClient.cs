namespace Unmatched.HttpClients.Contracts;

public interface IFavoriteClient
{
    Task<Guid?> GetFavouriteHeroIdAsync(Guid playerId);
}