namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

public class MatchHandlerFactory(
    IUnitOfWork unitOfWork,
    IRatingCalculator ratingCalculator,
    IFirstTournamentRatingCalculator firstTournamentRatingCalculator,
    IUnrankedRatingCalculator unrankedRatingCalculator) : IMatchHandlerFactory
{
    private IEnumerable<TournamentEntity> TournamentsCache => unitOfWork.Tournaments.Get();

    public IMatchHandler Create(MatchEntity match) => match switch
    {
        _ when IsUnranked(match) =>
            new UnrankedMatchHandler(unitOfWork, unrankedRatingCalculator),
        _ when IsFirstTournament(match) =>
            new FirstTournamentMatchHandler(unitOfWork, firstTournamentRatingCalculator),
        _ when IsGoldenHalatLeague(match) || IsSilverhandTournament(match) => 
            new GoldenHalatLeagueMatchHandler(unitOfWork, ratingCalculator),
        _ =>  new GoldenHalatLeagueMatchHandler(unitOfWork, ratingCalculator)
    };

    private static bool IsUnranked(MatchEntity match) 
        => match.TournamentId == null;
    
    private bool IsFirstTournament(MatchEntity match)
        => TournamentPredicateInternal(match, TournamentNames.UnmatchedFirstTournament);

    private bool IsGoldenHalatLeague(MatchEntity match)
        => TournamentPredicateInternal(match, TournamentNames.GoldenHalatLeague);

    private bool IsSilverhandTournament(MatchEntity match) 
        => TournamentPredicateInternal(match, TournamentNames.SilverhandTournament);

    private bool TournamentPredicateInternal(MatchEntity match, string targetTournamentName)
    {
        if (match.TournamentId is null)
        {
            return false;
        }

        var tournamentName = TournamentsCache.FirstOrDefault(x => x.Id.Equals(match.TournamentId))?.Name;
        return tournamentName == targetTournamentName;
    }
}
