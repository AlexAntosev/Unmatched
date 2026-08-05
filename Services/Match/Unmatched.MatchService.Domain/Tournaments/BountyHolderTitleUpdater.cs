namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>
/// Moves a Bounty tournament's dynamic "Bounty {Name} holder" title to whoever just won a challenge,
/// mirroring <see cref="Titles.TournamentTitleAwarder"/>'s per-tournament title rows but updated live
/// instead of once at completion (Bounty tournaments never complete). Kept separate from
/// <see cref="RatingCalculators.BountyRatingCalculator"/> - rating and title concerns are already
/// independent steps for every match (see <see cref="Services.RatingService.RecalculateAsync"/>
/// calling the match handler and the title evaluator separately).
/// </summary>
public class BountyHolderTitleUpdater(IUnitOfWork unitOfWork)
{
    /// <summary>No SaveChangesAsync here - the caller's single save (see
    /// <see cref="MatchHandlers.MatchHandler.HandleAsync"/>) commits this alongside the
    /// match/fighter/rating changes.</summary>
    public async Task UpdateAsync(TournamentEntity tournament, MatchEntity match)
    {
        var winner = match.Fighters.First(f => f.IsWinner);
        var title = await FindOrCreateTitleAsync(tournament);

        foreach (var stale in title.HeroTitles.Where(h => h.HeroesId != winner.HeroId).ToList())
        {
            title.HeroTitles.Remove(stale);
        }

        if (title.HeroTitles.All(h => h.HeroesId != winner.HeroId))
        {
            title.HeroTitles.Add(new HeroTitleEntity { HeroesId = winner.HeroId, TitlesId = title.Id, EarnedAt = match.Date });
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
