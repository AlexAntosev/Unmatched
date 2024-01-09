namespace Unmatched.Services.MatchHandlers;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
using Unmatched.Services.RatingCalculators;

public class UnrankedMatchHandler : BaseMatchHandler
{
    private readonly IUnrankedRatingCalculator _ratingCalculator;

    public UnrankedMatchHandler(IUnitOfWork unitOfWork, IUnrankedRatingCalculator ratingCalculator)
    : base(unitOfWork)
    {
        _ratingCalculator = ratingCalculator;
    }

    protected override async Task InnerHandleAsync(Match match)
    {
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last());
        
        await CreateMatch(match, matchPoints);
        
        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(Match match)
    {
    }
}
