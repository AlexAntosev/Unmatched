namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Validation;

/// <summary>
/// The single match-saving pipeline for every game mode: validate the shape of the match, work out
/// each hero's rating change (or none, for an Unranked match), persist the match, and apply the
/// rating changes. Game-mode-specific rating math lives in <see cref="IRatingCalculatorFactory"/>,
/// not here - this class only knows how to apply whatever a calculator returns.
/// </summary>
/// <remarks>
/// Replaces the old tournament-name-dispatched <c>MatchHandlerFactory</c> and its six near-identical
/// per-mode handlers: with one <see cref="IRatingCalculator"/> contract there is nothing left for a
/// factory of handlers to select between.
/// </remarks>
public class MatchHandler(
    IUnitOfWork unitOfWork,
    IGameModeValidatorFactory validatorFactory,
    IRatingCalculatorFactory ratingCalculatorFactory) : IMatchHandler
{
    public async Task HandleAsync(MatchEntity match)
    {
        validatorFactory.Create(match.GameMode).Validate(match);

        var matchPoints = match.IsRanked
            ? await ratingCalculatorFactory.Create(match.GameMode).CalculateAsync(match)
            : new Dictionary<Guid, int>();

        await CreateMatchAsync(match, matchPoints);

        await unitOfWork.SaveChangesAsync();
    }

    private async Task CreateMatchAsync(MatchEntity match, Dictionary<Guid, int> matchPoints)
    {
        foreach (var fighter in match.Fighters)
        {
            fighter.MatchPoints = matchPoints.TryGetValue(fighter.HeroId, out var points) ? points : 0;
        }

        match.IsPlanned = false;
        if (match.Id == Guid.Empty)
        {
            await unitOfWork.Matches.AddAsync(match);
        }
        else
        {
            unitOfWork.Matches.Update(match);
        }

        await RatingLedger.ApplyAsync(unitOfWork, matchPoints);
    }
}
