using Unmatched.Dtos;

namespace Unmatched.Services;

public interface IUnmatchedService
{
    Task AddDuelMatchAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto);
    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
    Task<IEnumerable<HeroDto>> GetAllHeroesAsync();
    Task<IEnumerable<MapDto>> GetAllMapsAsync();
    Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync();
}