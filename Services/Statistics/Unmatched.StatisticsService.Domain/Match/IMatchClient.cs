namespace Unmatched.StatisticsService.Domain.Match;

public interface IMatchClient
{
    Task<IEnumerable<FighterDto>> GetAllFightersAsync();

    Task<IEnumerable<RatingDto>> GetAllRatingsAsync();

    Task<IEnumerable<FighterDto>> GetFightersByHeroAsync(Guid heroId);

    Task<RatingDto> GetHeroRatingAsync(Guid heroId);
}