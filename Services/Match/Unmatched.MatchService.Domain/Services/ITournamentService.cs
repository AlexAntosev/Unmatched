namespace Unmatched.MatchService.Domain.Services;

using System;

using Unmatched.MatchService.Domain.Models;

public interface ITournamentService
{
    Task<Tournament> AddAsync(Tournament dto);

    Task<IEnumerable<Tournament>> GetAsync();

    Task<Tournament> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task CreateNextStagePlannedMatchesAsync(Guid tournamentId);

    Task<Tournament?> UpdateImageAsync(Guid id, string imageFileName);

    Task<Tournament?> UpdateTrophyImageAsync(Guid id, string imageFileName);

    Task<IEnumerable<TournamentStanding>> GetStandingsAsync(Guid tournamentId);

    Task<Tournament> CompleteAsync(Guid tournamentId);

    Task RecomputeCompletionAwardsAsync(Guid tournamentId);

    Task ReconcileCompletionAwardsAsync();

    Task<IEnumerable<TournamentAward>> GetAwardsAsync(Guid tournamentId);

    Task<BountyState> GetBountyStateAsync(Guid tournamentId);

    Task<Match> CreateBountyChallengeAsync(
        Guid tournamentId, Guid challengerHeroId, Guid championPlayerId, Guid challengerPlayerId, Guid mapId);
}
