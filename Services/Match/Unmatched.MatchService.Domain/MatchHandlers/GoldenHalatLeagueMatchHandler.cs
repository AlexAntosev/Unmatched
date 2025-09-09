namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

public class GoldenHalatLeagueMatchHandler : BaseMatchHandler
{
    private readonly IRatingCalculator _ratingCalculator;

    public GoldenHalatLeagueMatchHandler(IUnitOfWork unitOfWork, IRatingCalculator ratingCalculator)
    : base(unitOfWork)
    {
        _ratingCalculator = ratingCalculator;
    }
    
    protected override async Task InnerHandleAsync(MatchEntity match)
    {
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last());
        
        await CreateMatch(match, matchPoints);
        
        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(MatchEntity match)
    {
    }
}
