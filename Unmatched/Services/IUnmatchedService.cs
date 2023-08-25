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
    Task<IEnumerable<HeroStatisticsDto>> GetHeroesStatisticsAsync();
    Task<IEnumerable<PlayerStatisticsDto>> GetPlayersStatisticsAsync();
    Task<IEnumerable<MapStatisticsDto>> GetMapsStatisticsAsync();
    
    Task<HeroStatisticsDto> GetHeroStatisticsAsync(Guid heroId);
    Task<PlayerStatisticsDto> GetPlayerStatisticsAsync(Guid playerId);
    Task<MapStatisticsDto> GetMapStatisticsAsync(Guid mapId);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();
    
    Task<IEnumerable<MatchLogDto>> GetHeroMatchesAsync(Guid heroId);
    Task<IEnumerable<MatchLogDto>> GetPlayerMatchesAsync(Guid playerId);
    Task<IEnumerable<MatchLogDto>> GetMapMatchesAsync(Guid mapId);
}
