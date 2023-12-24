namespace Unmatched.Services.MatchHandlers;

using Unmatched.Entities;
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
        var matchPoints = (await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last())).ToArray();
        
        await CreateMatch(match, matchPoints);
        
        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(Match match)
    {
    }
}
