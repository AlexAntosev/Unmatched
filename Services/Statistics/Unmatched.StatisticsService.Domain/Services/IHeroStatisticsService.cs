namespace Unmatched.StatisticsService.Domain.Services;

using Unmatched.StatisticsService.Domain.Models;

public interface IHeroStatisticsService
{
    Task<IEnumerable<HeroStats>> GetHeroesStatisticsAsync();

    Task<HeroStats> GetHeroStatisticsAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId); // match

    Task<List<RatingChangeDto>> GetRatingChangesAsync(Guid heroId); // match
}
