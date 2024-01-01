namespace Unmatched.Services;

using Unmatched.Dtos;
using Unmatched.Entities;

public interface IMatchService
{
    public Task AddAsync(Match match);
    
    public Task AddAsync(MatchDto match, FighterDto fighter, FighterDto opponent);
    
    Task<IEnumerable<HeroDto>> GetAllHeroesAsync();

    Task<IEnumerable<MapDto>> GetAllMapsAsync();

    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();

    Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync();

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id, Stage? stage = null);
}
