namespace Unmatched.Services.MatchHandlers;

using Unmatched.Data.Entities;
using Unmatched.Data.Repositories;
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
        var matchPoints = await _ratingCalculator.CalculateAsync(match.Fighters.First(), match.Fighters.Last(), match.Stage.Value);
        
        await CreateMatch(match, matchPoints);

        await UnitOfWork.SaveChangesAsync();
    }

    protected override void InnerValidate(Match match)
    {
        if (match.Stage is null)
        {
            throw new InvalidCastException($"{nameof(match)} has no stage");
        }
    }
}
