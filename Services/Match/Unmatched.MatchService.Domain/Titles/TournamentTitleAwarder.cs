namespace Unmatched.MatchService.Domain.Titles;

using Unmatched.MatchService.Domain.Communication.Catalog;
using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Repositories;

/// <summary>
/// On tournament completion, resolves each title kind the tournament selected at creation to its holder
/// and assigns a tournament-scoped <see cref="TitleEntity"/> - one row per (tournament, kind), so two
/// tournaments' Champions coexist rather than sharing a holder set. Reads only persisted
/// <see cref="TournamentParticipantEntity.FinalPlacement"/> and match data, so it can be run
/// retroactively over a tournament that completed before this existed. Does not call
/// SaveChangesAsync itself - <see cref="Services.TournamentService.CompleteAsync"/> persists everything
/// completion touches (awards, placements, ratings, titles) in one transaction.
/// </summary>
public class TournamentTitleAwarder(IUnitOfWork unitOfWork, ICatalogHeroCache catalogHeroCache)
{
    public async Task AwardAsync(TournamentEntity tournament, IReadOnlyList<MatchEntity> matches)
    {
        foreach (var kind in tournament.TournamentTitles.Select(t => t.Kind).Distinct())
        {
            var holderId = await ResolveHolderAsync(kind, tournament, matches);
            if (holderId is null)
            {
                continue;
            }

            var title = await FindOrCreateTitleAsync(tournament, kind);
            if (title.HeroTitles.All(h => h.HeroesId != holderId))
            {
                title.HeroTitles.Add(new HeroTitleEntity
                {
                    HeroesId = holderId.Value,
                    TitlesId = title.Id,
                    EarnedAt = tournament.CompletedAt
                });
            }
        }
    }

    private Task<Guid?> ResolveHolderAsync(TournamentTitleKind kind, TournamentEntity tournament, IReadOnlyList<MatchEntity> matches)
        => kind switch
        {
            TournamentTitleKind.Champion => Task.FromResult(ByPlacement(tournament, 1)),
            TournamentTitleKind.RunnerUp => Task.FromResult(ByPlacement(tournament, 2)),
            TournamentTitleKind.IronChin => MostHpLostAsync(matches),
            TournamentTitleKind.CardShark => MostCardsUsedAsync(matches),
            TournamentTitleKind.Executioner => MostHpDealtAsync(matches),
            TournamentTitleKind.Cinderella => LowestRatedSemifinalistAsync(tournament),
            _ => Task.FromResult<Guid?>(null)
        };

    private static Guid? ByPlacement(TournamentEntity tournament, int placement)
        => tournament.Participants.FirstOrDefault(p => p.FinalPlacement == placement)?.HeroId;

    private async Task<Guid?> MostHpLostAsync(IReadOnlyList<MatchEntity> matches)
    {
        var totals = new Dictionary<Guid, int>();
        foreach (var fighter in matches.SelectMany(m => m.Fighters).Where(f => f.HpLeft.HasValue))
        {
            var hero = await catalogHeroCache.GetAsync(fighter.HeroId);
            var lost = Math.Max(0, hero!.Hp - fighter.HpLeft!.Value);
            totals[fighter.HeroId] = totals.GetValueOrDefault(fighter.HeroId) + lost;
        }

        return HighestOrNull(totals);
    }

    private async Task<Guid?> MostCardsUsedAsync(IReadOnlyList<MatchEntity> matches)
    {
        var totals = new Dictionary<Guid, int>();
        foreach (var fighter in matches.SelectMany(m => m.Fighters).Where(f => f.CardsLeft.HasValue))
        {
            var hero = await catalogHeroCache.GetAsync(fighter.HeroId);
            var used = Math.Max(0, hero!.DeckSize - fighter.CardsLeft!.Value);
            totals[fighter.HeroId] = totals.GetValueOrDefault(fighter.HeroId) + used;
        }

        return HighestOrNull(totals);
    }

    /// <summary>Only 1v1 matches are attributed - see ExecutionerTitleRule for why team/FFA matches are
    /// skipped rather than guessed at.</summary>
    private async Task<Guid?> MostHpDealtAsync(IReadOnlyList<MatchEntity> matches)
    {
        var totals = new Dictionary<Guid, int>();
        foreach (var oneVOne in matches.Where(m => m.Fighters.Count == 2))
        {
            var fighters = oneVOne.Fighters.ToList();
            var (a, b) = (fighters[0], fighters[1]);
            await CreditHpDealtAsync(totals, dealer: a.HeroId, target: b);
            await CreditHpDealtAsync(totals, dealer: b.HeroId, target: a);
        }

        return HighestOrNull(totals);
    }

    private async Task CreditHpDealtAsync(Dictionary<Guid, int> totals, Guid dealer, FighterEntity target)
    {
        if (target.HpLeft is null)
        {
            return;
        }

        var hero = await catalogHeroCache.GetAsync(target.HeroId);
        var dealt = Math.Max(0, hero!.Hp - target.HpLeft.Value);
        if (dealt <= 0)
        {
            return;
        }

        totals[dealer] = totals.GetValueOrDefault(dealer) + dealt;
    }

    /// <summary>"Reaching the semis" is generalised to a top-4 final placement so League and Swiss
    /// tournaments (which have no literal semifinal round) can select Cinderella too.</summary>
    private async Task<Guid?> LowestRatedSemifinalistAsync(TournamentEntity tournament)
    {
        Guid? lowestHeroId = null;
        var lowestRating = int.MaxValue;
        foreach (var participant in tournament.Participants.Where(p => p.FinalPlacement is >= 1 and <= 4))
        {
            var rating = (await unitOfWork.Ratings.GetByHeroIdAsync(participant.HeroId))?.Points ?? RatingConstants.InitialRating;
            if (rating < lowestRating || (rating == lowestRating && lowestHeroId.HasValue && participant.HeroId.CompareTo(lowestHeroId.Value) < 0))
            {
                lowestRating = rating;
                lowestHeroId = participant.HeroId;
            }
        }

        return lowestHeroId;
    }

    private static Guid? HighestOrNull(Dictionary<Guid, int> totals)
    {
        if (totals.Count == 0)
        {
            return null;
        }

        var max = totals.Values.Max();
        return max <= 0 ? null : totals.Where(kv => kv.Value == max).OrderBy(kv => kv.Key).First().Key;
    }

    private async Task<TitleEntity> FindOrCreateTitleAsync(TournamentEntity tournament, TournamentTitleKind kind)
    {
        var name = $"{KindLabel(kind)} of {tournament.Name}";
        var existingId = (await unitOfWork.Titles.GetAsync())
            .FirstOrDefault(t => t.TournamentId == tournament.Id && t.Name == name)?.Id;

        if (existingId is not null)
        {
            return (await unitOfWork.Titles.GetByIdAsync(existingId.Value))!;
        }

        var created = new TitleEntity
        {
            Name = name,
            Comment = KindDescription(kind),
            Exclusivity = TitleExclusivity.Unique,
            TournamentId = tournament.Id,
            HeroTitles = new List<HeroTitleEntity>()
        };
        return await unitOfWork.Titles.AddAsync(created);
    }

    private static string KindLabel(TournamentTitleKind kind) => kind switch
    {
        TournamentTitleKind.Champion => "Champion",
        TournamentTitleKind.RunnerUp => "Runner-Up",
        TournamentTitleKind.IronChin => "Iron Chin",
        TournamentTitleKind.CardShark => "Card Shark",
        TournamentTitleKind.Executioner => "Executioner",
        TournamentTitleKind.Cinderella => "Cinderella",
        _ => kind.ToString()
    };

    private static string KindDescription(TournamentTitleKind kind) => kind switch
    {
        TournamentTitleKind.Champion => "Won the tournament.",
        TournamentTitleKind.RunnerUp => "Lost the final.",
        TournamentTitleKind.IronChin => "Lost the most HP across the tournament.",
        TournamentTitleKind.CardShark => "Used the most cards across the tournament.",
        TournamentTitleKind.Executioner => "Dealt the most HP damage across the tournament.",
        TournamentTitleKind.Cinderella => "Lowest-rated hero to reach the semifinals.",
        _ => ""
    };
}
