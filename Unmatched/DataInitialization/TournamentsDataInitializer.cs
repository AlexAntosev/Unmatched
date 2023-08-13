using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

namespace Unmatched.DataInitialization;

public class TournamentsDataInitializer : IDataInitializer<Tournament>
{
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentsDataInitializer(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task InitializeAsync()
    {
        var defaultTournaments = GetEntities();
        
        await _tournamentRepository.AddRangeAsync(defaultTournaments);
        await _tournamentRepository.SaveChangesAsync();
    }

    public IEnumerable<Tournament> GetEntities()
    {
        return new List<Tournament>
        {
            new() { Name = TournamentNames.UnmatchedFirstTournament, Type = TournamentType.Championship },
            new() { Name = TournamentNames.GoldenHalatLeague, Type = TournamentType.League },
        };
    }
}