namespace Unmatched.Services.MatchHandlers;

using Microsoft.Extensions.Logging;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

public class MatchHandlerFactory
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly ITournamentRepository _tournamentRepository;

    private IEnumerable<Tournament>? _tournamentsCache;

    public MatchHandlerFactory(ILoggerFactory loggerFactory, ITournamentRepository tournamentRepository)
    {
        _loggerFactory = loggerFactory;
        _tournamentRepository = tournamentRepository;
    }

    private IEnumerable<Tournament> TournamentsCache => _tournamentsCache ??= _tournamentRepository.Query().ToList();

    public IMatchHandler Create(Match match)
    {
        IMatchHandler handler = new EmptyMatchHandler(_loggerFactory);
        if (IsUnranked(match))
        {
            handler = new UnrankedMatchHandler();
        }
        else if (IsFirstTournament(match))
        {
            handler = new FirstTournamentMatchHandler();
        }
        else if (IsGoldenHalatLeague(match))
        {
            handler = new GoldenHalatLeagueMatchHandler();
        }

        return handler;
    }

    private bool IsFirstTournament(Match match)
    {
        var result = false;
        if (match.TournamentId is not null)
        {
            var tournamentName = TournamentsCache.FirstOrDefault(x => x.Id.Equals(match.TournamentId))?.Name;
            result = tournamentName == TournamentNames.UnmatchedFirstTournament;
        }

        return result;
    }

    private bool IsGoldenHalatLeague(Match match)
    {
        var result = false;
        if (match.TournamentId is not null)
        {
            var tournamentName = TournamentsCache.FirstOrDefault(x => x.Id.Equals(match.TournamentId))?.Name;
            result = tournamentName == TournamentNames.GoldenHalatLeague;
        }

        return result;
    }

    private bool IsUnranked(Match match)
    {
        return match.TournamentId == null;
    }
}
