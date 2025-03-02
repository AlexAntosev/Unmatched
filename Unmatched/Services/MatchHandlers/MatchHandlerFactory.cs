namespace Unmatched.Services.MatchHandlers;

using Unmatched.Constants;
using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Services.RatingCalculators;

public class MatchHandlerFactory : IMatchHandlerFactory
{
    private readonly IFirstTournamentRatingCalculator _firstTournamentRatingCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRatingCalculator _ratingCalculator;
    private readonly IUnrankedRatingCalculator _unrankedRatingCalculator;

    public MatchHandlerFactory(
        IUnitOfWork unitOfWork,
        IRatingCalculator ratingCalculator,
        IFirstTournamentRatingCalculator firstTournamentRatingCalculator,
        IUnrankedRatingCalculator unrankedRatingCalculator)
    {
        _unitOfWork = unitOfWork;
        _ratingCalculator = ratingCalculator;
        _firstTournamentRatingCalculator = firstTournamentRatingCalculator;
        _unrankedRatingCalculator = unrankedRatingCalculator;
    }

    private IEnumerable<Tournament> TournamentsCache => _unitOfWork.Tournaments.Query(true).ToList();

    public IMatchHandler Create(Match match) => match switch
    {
        _ when IsUnranked(match) =>
            new UnrankedMatchHandler(_unitOfWork, _unrankedRatingCalculator),
        _ when IsFirstTournament(match) =>
            new FirstTournamentMatchHandler(_unitOfWork, _firstTournamentRatingCalculator),
        _ when IsGoldenHalatLeague(match) || IsSilverhandTournament(match) => 
            new GoldenHalatLeagueMatchHandler(_unitOfWork, _ratingCalculator),
        _ =>  new GoldenHalatLeagueMatchHandler(_unitOfWork, _ratingCalculator)
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

        var tournamentName = TournamentsCache.FirstOrDefault(x => x.Id.Equals(match.TournamentId))?.Name;
        return tournamentName == targetTournamentName;
    }
}
