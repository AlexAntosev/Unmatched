namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.RatingCalculators;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>
/// Bounty tournaments never complete (see <see cref="TournamentAwardScheduler"/>), so the pool pays out
/// per challenge instead: whoever wins a Bounty match earns a flat ELO bonus, recorded as a
/// <see cref="TournamentAwardEntity"/> - the same primitive tournament completion uses - so it replays
/// correctly through <see cref="Services.RatingTimeline"/> and shows up in the rating history chart.
/// Unlike a completion's one-time award batch, Bounty awards land one per challenge spread across many
/// dates, so <see cref="Services.RatingService.RecalculateAsync"/> replays each one individually via
/// <see cref="ReplayAsync"/> rather than batching by tournament. Winning also transfers the
/// tournament's dynamic "Bounty {Name} holder" title to the winner, mirroring
/// <see cref="Titles.TournamentTitleAwarder"/>'s per-tournament title rows but updated live instead of
/// once at completion.
/// </summary>
public class BountyChallengeResolver(IUnitOfWork unitOfWork)
{
    public async Task ResolveAsync(MatchEntity match)
    {
        if (match.TournamentId is null)
        {
            return;
        }

        var tournament = await unitOfWork.Tournaments.GetByIdAsync(match.TournamentId.Value);
        if (tournament is null || tournament.Format != TournamentFormat.Bounty)
        {
            return;
        }

        var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);
        if (winner is null)
        {
            return;
        }

        var award = new TournamentAwardEntity
        {
            TournamentId = tournament.Id,
            HeroId = winner.HeroId,
            AwardKind = TournamentAwardKind.BountyChallengeWin,
            Points = (int)Math.Round(RatingConstants.BountyChallengeWinAwardMultiplier * RatingConstants.KFactor),
            AwardedAt = match.Date
        };
        await unitOfWork.TournamentAwards.AddAsync(award);

        await ApplyAsync(tournament, award);
    }

    /// <summary>Re-applies an already-persisted Bounty award during a rating recalculation - see
    /// <see cref="Services.RatingService.RecalculateAsync"/>. Does not re-insert the award row.</summary>
    public async Task ReplayAsync(TournamentAwardEntity award)
    {
        var tournament = await unitOfWork.Tournaments.GetByIdAsync(award.TournamentId);
        if (tournament is null)
        {
            return;
        }

        await ApplyAsync(tournament, award);
    }

    private async Task ApplyAsync(TournamentEntity tournament, TournamentAwardEntity award)
    {
        await RatingLedger.ApplyAsync(unitOfWork, new Dictionary<Guid, int> { [award.HeroId] = award.Points });
        await UpdateHolderAsync(tournament, award.HeroId, award.AwardedAt);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task UpdateHolderAsync(TournamentEntity tournament, Guid championId, DateTime asOf)
    {
        var title = await FindOrCreateTitleAsync(tournament);

        foreach (var stale in title.HeroTitles.Where(h => h.HeroesId != championId).ToList())
        {
            title.HeroTitles.Remove(stale);
        }

        if (title.HeroTitles.All(h => h.HeroesId != championId))
        {
            title.HeroTitles.Add(new HeroTitleEntity { HeroesId = championId, TitlesId = title.Id, EarnedAt = asOf });
        }
    }

    private async Task<TitleEntity> FindOrCreateTitleAsync(TournamentEntity tournament)
    {
        var existing = (await unitOfWork.Titles.GetAsync()).FirstOrDefault(t => t.TournamentId == tournament.Id);
        if (existing is not null)
        {
            return (await unitOfWork.Titles.GetByIdAsync(existing.Id))!;
        }

        var created = new TitleEntity
        {
            Name = $"Bounty {tournament.Name} holder",
            Comment = "Won the last Bounty challenge for this pool.",
            Exclusivity = TitleExclusivity.Unique,
            TournamentId = tournament.Id,
            HeroTitles = new List<HeroTitleEntity>()
        };
        return await unitOfWork.Titles.AddAsync(created);
    }
}
