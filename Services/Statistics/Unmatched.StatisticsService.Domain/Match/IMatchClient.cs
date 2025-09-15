using Unmatched.StatisticsService.Domain.Match.Dto;

namespace Unmatched.StatisticsService.Domain.Match;

public interface IMatchClient
{
    Task<IEnumerable<HeroStatsFighterDto>> GetAllFightersAsync();

    Task<IEnumerable<RatingDto>> GetAllRatingsAsync();

    Task<IEnumerable<HeroStatsFighterDto>> GetFightersByHeroAsync(Guid heroId);

    Task<RatingDto> GetHeroRatingAsync(Guid heroId);

    Task<IEnumerable<MatchDto>> GetAllMatchesAsync();
}