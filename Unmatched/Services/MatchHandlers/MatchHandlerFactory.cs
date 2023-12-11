namespace Unmatched.Services.MatchHandlers;

using Microsoft.Extensions.Logging;
using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

public class MatchHandlerFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRatingCalculator _ratingCalculator;

    private readonly IFirstTournamentRatingCalculator _firstTournamentRatingCalculator;

    private readonly IMatchRepository _matchRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IFighterRepository _fighterRepository;

    private IEnumerable<Tournament>? _tournamentsCache;

    public MatchHandlerFactory(
        ILoggerFactory loggerFactory,
        ITournamentRepository tournamentRepository,
        IRatingCalculator ratingCalculator,
        IFirstTournamentRatingCalculator firstTournamentRatingCalculator,
        IMatchRepository matchRepository,
        IRatingRepository ratingRepository,
        IFighterRepository fighterRepository)
    {
        _loggerFactory = loggerFactory;
        _tournamentRepository = tournamentRepository;
        _ratingCalculator = ratingCalculator;
        _firstTournamentRatingCalculator = firstTournamentRatingCalculator;
        _matchRepository = matchRepository;
        _ratingRepository = ratingRepository;
        _fighterRepository = fighterRepository;
    }

    private IEnumerable<Tournament> TournamentsCache => _tournamentsCache ??= _tournamentRepository.Query().ToList();

    public IMatchHandler Create(Match match)
    {
        IMatchHandler handler = new EmptyMatchHandler(_loggerFactory);
        if (IsUnranked(match))
        {
            handler = new UnrankedMatchHandler(_matchRepository, _fighterRepository);
        }
        else if (IsFirstTournament(match))
        {
            handler = new FirstTournamentMatchHandler(_firstTournamentRatingCalculator, _matchRepository, _ratingRepository, _fighterRepository);
        }
        else if (IsGoldenHalatLeague(match))
        {
            handler = new GoldenHalatLeagueMatchHandler(_ratingCalculator, _matchRepository, _ratingRepository, _fighterRepository);
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
