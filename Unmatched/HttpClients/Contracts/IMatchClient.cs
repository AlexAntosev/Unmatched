namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface IMatchClient
{
    Task<SaveMatchResultDto> AddAsync(MatchDto match);

    Task<TournamentDto> AddTournamentAsync(TournamentDto tournament);

    Task GenerateTournamentNextStageAsync(Guid tournamentId);

    Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync();

    Task<MatchDto> GetAsync(Guid id);

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id);

    Task<IEnumerable<MatchLogDto>> GetFinishedByHeroAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetFinishedByMapAsync(Guid mapId);

    Task<IEnumerable<MatchLogDto>> GetFinishedByPlayerAsync(Guid playerId);

    Task<IEnumerable<RatingChangeDto>> GetHeroRatingChangesAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<TournamentDto> GetTournamentAsync(Guid id);

    Task RecalculateAsync();

    Task<SaveMatchResultDto> UpdateAsync(MatchDto match);

    Task UpdateEpicAsync(Guid matchId, int epic);
}
