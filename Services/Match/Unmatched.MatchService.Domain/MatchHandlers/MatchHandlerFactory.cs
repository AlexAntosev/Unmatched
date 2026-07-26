namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

public class MatchHandlerFactory(
    IUnitOfWork unitOfWork,
    IGameModeValidatorFactory validatorFactory,
    IRatingCalculator ratingCalculator,
    IFirstTournamentRatingCalculator firstTournamentRatingCalculator,
    IUnrankedRatingCalculator unrankedRatingCalculator,
    ITeamVsTeamRatingCalculator teamVsTeamRatingCalculator,
    IFreeForAllRatingCalculator freeForAllRatingCalculator,
    ICooperativeRatingCalculator cooperativeRatingCalculator) : IMatchHandlerFactory
{
    private IEnumerable<TournamentEntity> TournamentsCache => unitOfWork.Tournaments.Get();

    public IMatchHandler Create(MatchEntity match) => match switch
    {
        _ when match.GameMode == GameMode.TeamVsTeam =>
            new TeamVsTeamMatchHandler(unitOfWork, validatorFactory, teamVsTeamRatingCalculator),
        _ when match.GameMode == GameMode.FreeForAll =>
            new FreeForAllMatchHandler(unitOfWork, validatorFactory, freeForAllRatingCalculator),
        _ when match.GameMode == GameMode.Cooperative =>
            new CooperativeMatchHandler(unitOfWork, validatorFactory, cooperativeRatingCalculator),
        _ when IsUnranked(match) =>
            new UnrankedMatchHandler(unitOfWork, validatorFactory, unrankedRatingCalculator),
        _ when IsFirstTournament(match) =>
            new FirstTournamentMatchHandler(unitOfWork, validatorFactory, firstTournamentRatingCalculator),
        _ when IsGoldenHalatLeague(match) || IsSilverhandTournament(match) =>
            new GoldenHalatLeagueMatchHandler(unitOfWork, validatorFactory, ratingCalculator),
        _ =>  new GoldenHalatLeagueMatchHandler(unitOfWork, validatorFactory, ratingCalculator)
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
