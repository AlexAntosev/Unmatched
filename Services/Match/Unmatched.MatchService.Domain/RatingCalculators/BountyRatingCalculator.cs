namespace Unmatched.MatchService.Domain.RatingCalculators;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>
/// The standard 1v1 Elo delta N, split between the champion's title-holding pool and whoever just
/// fought for it: a successful defender keeps N/2 and banks the remainder; a challenger who dethrones
/// the champion takes N/2 plus the whole bank accumulated during the old reign, and the bank
/// immediately refills with N/2 under the new holder rather than resetting to 0. Either way the
/// defeated fighter loses the full N. Composes <see cref="OneVsOneRatingCalculator"/> for the Elo math
/// (a Bounty challenge is always a duel regardless of <see cref="MatchEntity.GameMode"/>) rather than
/// duplicating it.
/// </summary>
/// <remarks>
/// SIDE EFFECT beyond the <see cref="IRatingCalculator"/> contract: stages an upsert of this
/// tournament's single <see cref="TournamentAwardKind.BountyPool"/> row via
/// <c>unitOfWork.TournamentAwards.AddOrUpdateAsync</c>. Does not call SaveChangesAsync itself -
/// <see cref="MatchHandlers.MatchHandler.HandleAsync"/>'s single save commits it alongside the
/// match/fighter/rating changes in one transaction. Every other <see cref="IRatingCalculator"/> is a
/// pure read; this one is not.
/// </remarks>
public class BountyRatingCalculator(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache) : IRatingCalculator
{
    private readonly OneVsOneRatingCalculator _oneVsOne = new(unitOfWork, catalogHeroCache);

    public async Task<Dictionary<Guid, int>> CalculateAsync(MatchEntity match)
    {
        var tournamentId = match.TournamentId!.Value;
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(tournamentId);
        var poolRow = (await unitOfWork.TournamentAwards.GetByTournamentAsync(tournamentId))
            .FirstOrDefault(a => a.AwardKind == TournamentAwardKind.BountyPool);

        var currentHolderId = poolRow?.HeroId ?? tournament?.StartingChampionId;
        var bankBefore = poolRow?.Points ?? 0;

        var winner = match.Fighters.First(f => f.IsWinner);
        var loser = match.Fighters.First(f => !f.IsWinner);

        var n = (await _oneVsOne.CalculateAsync(match))[winner.HeroId];
        var winnerShare = n / 2;

        var isDefense = currentHolderId is not null && currentHolderId == winner.HeroId;
        var winnerPoints = isDefense ? winnerShare : winnerShare + bankBefore;
        var newBank = isDefense ? bankBefore + (n - winnerShare) : n - winnerShare;

        await unitOfWork.TournamentAwards.AddOrUpdateAsync(new TournamentAwardEntity
        {
            Id = poolRow?.Id ?? Guid.NewGuid(),
            TournamentId = tournamentId,
            HeroId = winner.HeroId,
            AwardKind = TournamentAwardKind.BountyPool,
            Points = newBank,
            AwardedAt = match.Date
        });

        return new Dictionary<Guid, int> { [winner.HeroId] = winnerPoints, [loser.HeroId] = -n };
    }
}
