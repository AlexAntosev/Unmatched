namespace Unmatched.Services.Contracts;

using System;
using Unmatched.Dtos.Match;

public interface ITournamentService
{
    Task<TournamentDto> AddAsync(TournamentDto dto);

    Task<IEnumerable<TournamentDto>> GetAsync();
    
    Task<TournamentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task GenerateMatchesAsync(Guid tournamentId);

    Task<IEnumerable<TournamentStandingDto>> GetStandingsAsync(Guid tournamentId);

    Task<TournamentDto> UpdateImageAsync(Guid id, string imageFileName);

    Task<TournamentDto> UpdateTrophyImageAsync(Guid id, string imageFileName);

    Task<TournamentDto> CompleteAsync(Guid id);

    Task<IEnumerable<TournamentAwardDto>> GetAwardsAsync(Guid id);

    Task<BountyStateDto> GetBountyStateAsync(Guid tournamentId);

    Task<MatchDto> CreateBountyChallengeAsync(Guid tournamentId, CreateBountyChallengeRequestDto request);
}