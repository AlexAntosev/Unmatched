using Unmatched.Services.Contracts;

namespace Unmatched.Services;

using Unmatched.Dtos.Match;
using Unmatched.HttpClients.Contracts;

public class TournamentService(IMatchClient matchClient) : ITournamentService
{
    public Task<TournamentDto> AddAsync(TournamentDto dto)
    {
        throw new NotImplementedException();
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

    public Task CreateInitialPlannedMatchesAsync(TournamentDto tournament)
    {
        throw new NotImplementedException();
    }

    public Task CreateNextStagePlannedMatchesAsync(TournamentDto tournament)
    {
        throw new NotImplementedException();
    }

    public Task CreateThirdPlaceDeciderMatchAsync(TournamentDto tournament)
    {
        throw new NotImplementedException();
    }

    public Task CreateGrandFinalMatchesAsync(TournamentDto tournament)
    {
        throw new NotImplementedException();
    }
}
