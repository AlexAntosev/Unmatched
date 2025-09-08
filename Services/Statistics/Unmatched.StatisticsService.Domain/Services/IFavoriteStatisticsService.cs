namespace Unmatched.StatisticsService.Domain.Services;

using System;

public interface IFavoriteStatisticsService
{
    Task<IEnumerable<FavoriteStatisticsDto>> GetFavoritesStatisticsAsync(Guid playerId);

    Task AddOrUpdateAsync(FavoriteStatisticsDto favoriteStatisticsDto);
}
