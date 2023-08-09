using Unmatched.Dtos;

namespace Unmatched.Services;

public interface IUnmatchedService
{
    Task AddDuelMatchAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto);
    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();
    
    IEnumerable<PlayerDto> GetAllPlayers();
    IEnumerable<HeroDto> GetAllHeroes();
    IEnumerable<MapDto> GetAllMaps();
    IEnumerable<TournamentDto> GetAllTournaments();
}