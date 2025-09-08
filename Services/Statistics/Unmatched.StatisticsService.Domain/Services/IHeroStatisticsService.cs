namespace Unmatched.StatisticsService.Domain.Services;

public interface IHeroStatisticsService
{
    Task<IEnumerable<HeroStatisticsDto>> GetHeroesStatisticsAsync();

    Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId);

    Task<List<RatingChangeDto>> GetRatingChangesAsync(Guid heroId);
}
