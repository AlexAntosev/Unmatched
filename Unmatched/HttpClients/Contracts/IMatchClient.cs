namespace Unmatched.HttpClients.Contracts;

using Unmatched.Dtos;
using Unmatched.Dtos.Match;

public interface IMatchClient
{
    Task<SaveMatchResultDto> AddAsync(MatchDto match);

    Task<TournamentDto> AddTournamentAsync(TournamentDto tournament);

    Task DeleteTournamentAsync(Guid id);

    Task GenerateTournamentNextStageAsync(Guid tournamentId);

    Task<IEnumerable<TournamentDto>> GetAllTournamentsAsync();

    Task<MatchDto> GetAsync(Guid id);

    Task<IEnumerable<MatchDto>> GetByTournamentIdAsync(Guid id);

    Task<IEnumerable<MatchLogDto>> GetFinishedByHeroAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetFinishedByMapAsync(Guid mapId);

    Task<IEnumerable<MatchLogDto>> GetFinishedByPlayerAsync(Guid playerId);

    Task<IEnumerable<MatchLogDto>> GetFinishedByVillainAsync(Guid villainId);

    Task<IEnumerable<MatchLogDto>> GetFinishedByMinionAsync(Guid minionId);

    Task<IEnumerable<RatingChangeDto>> GetHeroRatingChangesAsync(Guid heroId);

    Task<IEnumerable<MatchLogDto>> GetMatchLogAsync();

    Task<TournamentDto> GetTournamentAsync(Guid id);

    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid id);

    Task<TournamentDto> UpdateTournamentImageAsync(Guid id, string imageFileName);

    Task<TournamentDto> UpdateTournamentTrophyImageAsync(Guid id, string imageFileName);

    Task<TournamentDto> CompleteTournamentAsync(Guid id);

    Task<IEnumerable<TournamentAwardDto>> GetTournamentAwardsAsync(Guid id);

    Task RecalculateAsync();

    Task<bool> IsRatingRecalculationRequiredAsync();

    Task AddTitleAsync(TitleDto title);

    Task<IEnumerable<TitleDto>> GetTitlesAsync();

    Task<IEnumerable<TitleDto>> GetTitlesByHeroAsync(Guid heroId);

    Task DeleteTitleAsync(Guid id);

    Task MergeTitleAsync(Guid titleId, IEnumerable<Guid> heroesIds);

    Task<IEnumerable<HeroTitleAssignDto>> GetHeroesForTitleAssignAsync(Guid titleId);

    Task<SaveMatchResultDto> UpdateAsync(MatchDto match);

    Task UpdateEpicAsync(Guid matchId, int epic);
}
