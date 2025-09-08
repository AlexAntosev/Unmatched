namespace Unmatched.Services.Statistics;

using System;

using Unmatched.Dtos;

public interface IFavoriteStatisticsService
{
    Task<IEnumerable<FavoriteStatisticsDto>> GetFavoritesStatisticsAsync(Guid playerId);

    Task AddOrUpdateAsync(FavoriteStatisticsDto favoriteStatisticsDto);
}
