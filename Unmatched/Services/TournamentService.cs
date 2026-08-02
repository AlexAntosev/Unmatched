using Unmatched.Services.Contracts;

namespace Unmatched.Services;

using Unmatched.Dtos.Match;
using Unmatched.HttpClients.Contracts;

public class TournamentService(IMatchClient matchClient) : ITournamentService
{
    public Task<TournamentDto> AddAsync(TournamentDto dto)
    {
        return matchClient.AddTournamentAsync(dto);
    }

    public Task<IEnumerable<TournamentDto>> GetAsync()
    {
        return matchClient.GetAllTournamentsAsync();
    }

    public Task<TournamentDto> GetAsync(Guid id)
    {
        return matchClient.GetTournamentAsync(id);
    }

    public Task DeleteAsync(Guid id)
    {
        return matchClient.DeleteTournamentAsync(id);
    }

    public Task GenerateMatchesAsync(Guid tournamentId)
    {
        return matchClient.GenerateTournamentNextStageAsync(tournamentId);
    }

    public Task<IEnumerable<TournamentStandingDto>> GetStandingsAsync(Guid tournamentId)
    {
        return matchClient.GetTournamentStandingsAsync(tournamentId);
    }

    public Task<TournamentDto> UpdateImageAsync(Guid id, string imageFileName)
    {
        return matchClient.UpdateTournamentImageAsync(id, imageFileName);
    }

    public Task<TournamentDto> UpdateTrophyImageAsync(Guid id, string imageFileName)
    {
        return matchClient.UpdateTournamentTrophyImageAsync(id, imageFileName);
    }

    public Task<TournamentDto> CompleteAsync(Guid id)
    {
        return matchClient.CompleteTournamentAsync(id);
    }

    public Task<IEnumerable<TournamentAwardDto>> GetAwardsAsync(Guid id)
    {
        return matchClient.GetTournamentAwardsAsync(id);
    }
}
