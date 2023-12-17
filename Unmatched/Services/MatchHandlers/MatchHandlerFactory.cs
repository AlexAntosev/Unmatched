namespace Unmatched.Services.MatchHandlers;

using Microsoft.Extensions.Logging;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class MatchHandlerFactory : IMatchHandlerFactory
{
    private readonly IFirstTournamentRatingCalculator _firstTournamentRatingCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IRatingCalculator _ratingCalculator;
    private readonly IUnrankedRatingCalculator _unrankedRatingCalculator;

    private IEnumerable<Tournament>? _tournamentsCache;

    public MatchHandlerFactory(
        IUnitOfWork unitOfWork,
        ILoggerFactory loggerFactory,
        IRatingCalculator ratingCalculator,
        IFirstTournamentRatingCalculator firstTournamentRatingCalculator,
        IUnrankedRatingCalculator unrankedRatingCalculator)
    {
        _unitOfWork = unitOfWork;
        _loggerFactory = loggerFactory;
        _ratingCalculator = ratingCalculator;
        _firstTournamentRatingCalculator = firstTournamentRatingCalculator;
        _unrankedRatingCalculator = unrankedRatingCalculator;
    }

    private Lazy<IEnumerable<Tournament>> TournamentsCache => new(_unitOfWork.Tournaments.Query().ToList());

    public IMatchHandler Create(Match match) => match switch
    {
        _ when IsUnranked(match) =>
            new UnrankedMatchHandler(_unitOfWork, _unrankedRatingCalculator),
        _ when IsFirstTournament(match) =>
            new FirstTournamentMatchHandler(_unitOfWork, _firstTournamentRatingCalculator),
        _ when IsGoldenHalatLeague(match) || IsSilverhandTournament(match) => 
            new GoldenHalatLeagueMatchHandler(_unitOfWork, _ratingCalculator),
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
