namespace Unmatched.Services.Statistics;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface IHeroStatisticsService
{
    Task<IEnumerable<HeroStatisticsDto>> GetHeroesStatisticsAsync();

    Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId);

    Task<IEnumerable<UiMatchLogDto>> GetHeroMatchesAsync(Guid heroId);

    Task<List<RatingChangeDto>> GetRatingChangesAsync(Guid heroId);
}
