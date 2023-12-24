namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class FirstTournamentMatchHandler : BaseMatchHandler
{
    private readonly IFirstTournamentRatingCalculator _ratingCalculator;

    public FirstTournamentMatchHandler(IUnitOfWork unitOfWork, IFirstTournamentRatingCalculator ratingCalculator)
    : base(unitOfWork)
    {
        _ratingCalculator = ratingCalculator;
    }
    
    protected override async Task InnerHandleAsync(Match match)
    {
        var matchWithStage = (MatchWithStage)match;
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last(), matchWithStage.Stage);
        
        var createdMatch = await CreateMatch(match, matchPoints);

        await CreateMatchStageAsync(matchWithStage.Stage, createdMatch.Id);

        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(Match match)
    {
        if (match is not MatchWithStage)
        {
            throw new InvalidCastException($"{nameof(match)} is not of type {nameof(MatchWithStage)}");
        }
    }

    private Task<MatchStage> CreateMatchStageAsync(Stage stage, Guid matchId)
    {
        var matchStage = new MatchStage
            {
                MatchId = matchId,
                Stage = stage
            };
        return UnitOfWork.MatchStages.AddAsync(matchStage);
    }
}
