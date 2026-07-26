namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

public class FirstTournamentMatchHandler : BaseMatchHandler
{
    private readonly IFirstTournamentRatingCalculator _ratingCalculator;

    public FirstTournamentMatchHandler(IUnitOfWork unitOfWork, IGameModeValidatorFactory validatorFactory, IFirstTournamentRatingCalculator ratingCalculator)
    : base(unitOfWork, validatorFactory)
    {
        _ratingCalculator = ratingCalculator;
    }
    
    protected override async Task InnerHandleAsync(MatchEntity match)
    {
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last(), match.Stage.Value);
        
        await CreateMatch(match, matchPoints);

        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(MatchEntity match)
    {
        if (match.Stage is null)
        {
            throw new InvalidCastException($"{nameof(match)} has no stage");
        }
    }
}
