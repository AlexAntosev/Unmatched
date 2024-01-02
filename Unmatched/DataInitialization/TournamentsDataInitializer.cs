namespace Unmatched.DataInitialization;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Enums;
using Unmatched.Repositories;

public class TournamentsDataInitializer : IDataInitializer<Tournament>
{
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentsDataInitializer(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public IEnumerable<Tournament> GetEntities()
    {
        return new List<Tournament>
            {
                new()
                    {
                        Name = TournamentNames.UnmatchedFirstTournament,
                        Type = TournamentType.Championship
                    },
                new()
                    {
                        Name = TournamentNames.GoldenHalatLeague,
                        Type = TournamentType.League
                    },
                new()
                    {
                        Name = TournamentNames.SilverhandTournament,
                        Type = TournamentType.Championship
                    },
            };
    }

    public async Task InitializeAsync()
    {
        var defaultTournaments = GetEntities();

        await _tournamentRepository.AddRangeAsync(defaultTournaments);
        await _tournamentRepository.SaveChangesAsync();
    }
}
