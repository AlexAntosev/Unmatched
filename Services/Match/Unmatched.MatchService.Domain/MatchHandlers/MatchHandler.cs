namespace Unmatched.MatchService.Domain.MatchHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;
using Unmatched.MatchService.Domain.Tournaments;
using Unmatched.MatchService.Domain.Validation;

/// <summary>
/// The single match-saving pipeline for every game mode: validate the shape of the match, work out
/// each hero's rating change (or none, for an Unranked match), persist the match, and apply the
/// rating changes. Game-mode-specific rating math lives in <see cref="IRatingCalculatorFactory"/>,
/// not here - this class only knows how to apply whatever a calculator returns. The one exception is
/// a Bounty tournament match: it always uses <see cref="BountyRatingCalculator"/> instead of the
/// factory's game-mode dispatch, and (since that calculator's payout also means "this challenge was
/// resolved") triggers the tournament's holder-title update right alongside it.
/// </summary>
/// <remarks>
/// Replaces the old tournament-name-dispatched <c>MatchHandlerFactory</c> and its six near-identical
/// per-mode handlers: with one <see cref="IRatingCalculator"/> contract there is nothing left for a
/// factory of handlers to select between.
/// </remarks>
public class MatchHandler(
    IUnitOfWork unitOfWork,
    IGameModeValidatorFactory validatorFactory,
    IRatingCalculatorFactory ratingCalculatorFactory,
    BountyRatingCalculator bountyRatingCalculator,
    BountyHolderTitleUpdater bountyHolderTitleUpdater) : IMatchHandler
{
    public async Task HandleAsync(MatchEntity match)
    {
        validatorFactory.Create(match.GameMode).Validate(match);

        // A single tournament lookup serves both "which calculator" and "does the holder title need
        // updating" - an Unranked match (including an Unranked Bounty challenge) affects neither the
        // pool nor the title nor any rating, same as any other match type.
        TournamentEntity? tournament = null;
        var isBounty = false;
        if (match.IsRanked && match.TournamentId is not null)
        {
            tournament = await unitOfWork.Tournaments.GetByIdAsync(match.TournamentId.Value);
            isBounty = tournament?.Format == TournamentFormat.Bounty;
        }

        var matchPoints = match.IsRanked
            ? await (isBounty ? (IRatingCalculator)bountyRatingCalculator : ratingCalculatorFactory.Create(match.GameMode)).CalculateAsync(match)
            : new Dictionary<Guid, int>();

        await CreateMatchAsync(match, matchPoints);

        if (isBounty)
        {
            await bountyHolderTitleUpdater.UpdateAsync(tournament!, match);
        }

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
