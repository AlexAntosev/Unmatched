namespace Unmatched.Services;

using Unmatched.Dtos;

public interface IUnmatchedService
{
    Task AddDuelMatchAsync(MatchDto matchDto, FighterDto fighterDto, FighterDto opponentDto);

    Task<IEnumerable<HeroDto>> GetAllHeroesAsync();

    Task<IEnumerable<MapDto>> GetAllMapsAsync();

    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();

    Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync();

    Task<IEnumerable<RankedRatingHeroDto>> GetRankedRatingAsync();

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();
}
