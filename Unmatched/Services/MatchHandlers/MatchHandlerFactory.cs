namespace Unmatched.Services.MatchHandlers;

using Microsoft.Extensions.Logging;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class MatchHandlerFactory : IMatchHandlerFactory
{
    private readonly IFighterRepository _fighterRepository;

    private readonly IFirstTournamentRatingCalculator _firstTournamentRatingCalculator;

    private readonly ILoggerFactory _loggerFactory;

    private readonly IMatchRepository _matchRepository;

    private readonly IMatchStageRepository _matchStageRepository;

    private readonly IRatingCalculator _ratingCalculator;

    private readonly IRatingRepository _ratingRepository;

    private readonly ITournamentRepository _tournamentRepository;

    private readonly IUnrankedRatingCalculator _unrankedRatingCalculator;

    private IEnumerable<Tournament>? _tournamentsCache;

    public MatchHandlerFactory(
        ILoggerFactory loggerFactory,
        ITournamentRepository tournamentRepository,
        IRatingCalculator ratingCalculator,
        IFirstTournamentRatingCalculator firstTournamentRatingCalculator,
        IMatchRepository matchRepository,
        IRatingRepository ratingRepository,
        IFighterRepository fighterRepository,
        IMatchStageRepository matchStageRepository,
        IUnrankedRatingCalculator unrankedRatingCalculator)
    {
        _loggerFactory = loggerFactory;
        _tournamentRepository = tournamentRepository;
        _ratingCalculator = ratingCalculator;
        _firstTournamentRatingCalculator = firstTournamentRatingCalculator;
        _matchRepository = matchRepository;
        _ratingRepository = ratingRepository;
        _fighterRepository = fighterRepository;
        _matchStageRepository = matchStageRepository;
        _unrankedRatingCalculator = unrankedRatingCalculator;
    }

    private IEnumerable<Tournament> TournamentsCache => _tournamentsCache ??= _tournamentRepository.Query().ToList();

    public IMatchHandler Create(Match match)
    {
        IMatchHandler handler = new EmptyMatchHandler(_loggerFactory);
        if (IsUnranked(match))
        {
            handler = new UnrankedMatchHandler(_matchRepository, _fighterRepository, _unrankedRatingCalculator, _ratingRepository);
        }
        else if (IsFirstTournament(match))
        {
            handler = new FirstTournamentMatchHandler(_firstTournamentRatingCalculator, _matchRepository, _ratingRepository, _fighterRepository, _matchStageRepository);
        }
        else if (IsGoldenHalatLeague(match)
              || IsSilverhandTournament(match))
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

    private bool IsSilverhandTournament(Match match)
    {
        var result = false;
        if (match.TournamentId is not null)
        {
            var tournamentName = TournamentsCache.FirstOrDefault(x => x.Id.Equals(match.TournamentId))?.Name;
            result = tournamentName == TournamentNames.SilverhandTournament;
        }

        return result;
    }

    private bool IsUnranked(Match match)
    {
        return match.TournamentId == null;
    }
}
