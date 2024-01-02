namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
using Unmatched.Enums;
using Unmatched.Repositories;
using Unmatched.Services.RatingCalculators;

public class GoldenHalatLeagueMatchHandler : BaseMatchHandler
{
    private readonly IRatingCalculator _ratingCalculator;

    public GoldenHalatLeagueMatchHandler(IUnitOfWork unitOfWork, IRatingCalculator ratingCalculator)
    : base(unitOfWork)
    {
        _ratingCalculator = ratingCalculator;
    }
    
    protected override async Task InnerHandleAsync(Match match)
    {
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last());
        
        var createdMatch = await CreateMatch(match, matchPoints);

        if (match is MatchWithStage matchWithStage)
        {
            await CreateMatchStageAsync(matchWithStage.Stage, createdMatch.Id);
        }
        
        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(Match match)
    {
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
