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

    private Lazy<IEnumerable<Tournament>> TournamentsCache => new(_tournamentRepository.Query().ToList());

    public IMatchHandler Create(Match match) => match switch
    {
        _ when IsUnranked(match) =>
            new UnrankedMatchHandler(_matchRepository, _fighterRepository, _unrankedRatingCalculator, _ratingRepository),
        _ when IsFirstTournament(match) =>
            new FirstTournamentMatchHandler(_firstTournamentRatingCalculator, _matchRepository, _ratingRepository, _fighterRepository, _matchStageRepository),
        _ when IsGoldenHalatLeague(match) || IsSilverhandTournament(match) => 
            new GoldenHalatLeagueMatchHandler(_ratingCalculator, _matchRepository, _ratingRepository, _fighterRepository),
        _ =>  new EmptyMatchHandler(_loggerFactory)
    };

    private static bool IsUnranked(Match match) 
        => match.TournamentId == null;
    
    private bool IsFirstTournament(Match match)
        => TournamentPredicateInternal(match, TournamentNames.UnmatchedFirstTournament);

    private bool IsGoldenHalatLeague(Match match)
        => TournamentPredicateInternal(match, TournamentNames.GoldenHalatLeague);

    private bool IsSilverhandTournament(Match match) 
        => TournamentPredicateInternal(match, TournamentNames.SilverhandTournament);

    private bool TournamentPredicateInternal(Match match, string targetTournamentName)
    {
        if (match.TournamentId is null)
        {
            return false;
        }

        var tournamentName = TournamentsCache.Value.FirstOrDefault(x => x.Id.Equals(match.TournamentId))?.Name;
        return tournamentName == targetTournamentName;
    }
}
