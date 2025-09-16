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
        throw new NotImplementedException();
    }

    public Task GenerateMatchesAsync(Guid tournamentId)
    {
        return matchClient.GenerateTournamentNextStageAsync(tournamentId);
    }
}
