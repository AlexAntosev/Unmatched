namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

public class FreeForAllMatchHandler : BaseMatchHandler
{
    private readonly IFreeForAllRatingCalculator _ratingCalculator;

    public FreeForAllMatchHandler(IUnitOfWork unitOfWork, IGameModeValidatorFactory validatorFactory, IFreeForAllRatingCalculator ratingCalculator)
    : base(unitOfWork, validatorFactory)
    {
        _ratingCalculator = ratingCalculator;
    }

    protected override async Task InnerHandleAsync(MatchEntity match)
    {
        var matchPoints = await _ratingCalculator.CalculateAsync(match);

        await CreateMatch(match, matchPoints);

        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(MatchEntity match)
    {
    }
}
