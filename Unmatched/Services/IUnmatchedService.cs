namespace Unmatched.Services;

using Unmatched.Dtos;

public interface IUnmatchedService
{
    Task AddDuelMatchAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto);
    
    Task<HeroDto> GetHeroByIdAsync(Guid heroId);

    Task<IEnumerable<HeroDto>> GetAllHeroesAsync();

    Task<IEnumerable<MapDto>> GetAllMapsAsync();

    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();

    Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync();

    Task<IEnumerable<RankedRatingHeroStatisticsDto>> GetRankedRatingAsync();
    Task<IEnumerable<HeroStatisticsDto>> GetStatisticsAsync();
    
    Task<HeroStatisticsDto> GetStatisticsByHeroIdAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();
    
    Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId);
}
