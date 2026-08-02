namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Constants;
using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;

/// <summary>
/// Computes a completed tournament's placement bonuses. Every format's payout is expressed as a
/// multiple of <see cref="RatingConstants.KFactor"/> and then run through a self-funded pool: each
/// participant "pays" pool/N on entry, so the tournament's awards always sum to exactly zero and a
/// payout never inflates the closed hero ladder. See the plan's "Tournament completion bonuses" section
/// for the worked examples this class is built to reproduce.
/// </summary>
public class TournamentAwardScheduler
{
    public TournamentCompletionSchedule Schedule(
        TournamentEntity tournament, IReadOnlyList<MatchEntity> finishedMatches, DateTime awardedAt)
    {
        var participants = tournament.Participants.ToList();
        var (gross, placements) = tournament.Format switch
        {
            TournamentFormat.SingleElimination => BracketSchedule(finishedMatches),
            TournamentFormat.GroupStage => GroupStageSchedule(finishedMatches),
            TournamentFormat.Swiss => SwissSchedule(participants, finishedMatches),
            TournamentFormat.League => LeagueSchedule(participants, finishedMatches),
            _ => throw new InvalidOperationException($"{tournament.Format} tournaments don't support completion bonuses.")
        };

        foreach (var participant in participants)
        {
            if (!gross.ContainsKey(participant.HeroId))
            {
                gross[participant.HeroId] = [(TournamentAwardKind.Eliminated, 0.0)];
            }

            placements.TryAdd(participant.HeroId, null);
        }

        var awards = BuildAwardRows(tournament.Id, participants.Count, gross, awardedAt);
        return new TournamentCompletionSchedule(awards, placements);
    }

    private static List<TournamentAwardEntity> BuildAwardRows(
        Guid tournamentId,
        int participantCount,
        Dictionary<Guid, List<(TournamentAwardKind Kind, double Multiplier)>> grossByHero,
        DateTime awardedAt)
    {
        var scale = Math.Clamp(
            (double)participantCount / RatingConstants.TournamentAwardBaseParticipants,
            RatingConstants.TournamentAwardScaleMin,
            RatingConstants.TournamentAwardScaleMax);

        var scaledByHero = grossByHero.ToDictionary(
            kv => kv.Key,
            kv => kv.Value
                .Select(e => (e.Kind, Points: (int)Math.Round(e.Multiplier * RatingConstants.KFactor * scale)))
                .OrderByDescending(e => e.Points)
                .ToList());

        var pool = scaledByHero.Values.SelectMany(rows => rows).Sum(r => r.Points);
        var entry = participantCount == 0 ? 0 : pool / participantCount;
        var remainder = pool - entry * participantCount;

        var awards = new List<TournamentAwardEntity>();
        foreach (var (heroId, rows) in scaledByHero)
        {
            for (var i = 0; i < rows.Count; i++)
            {
                // the entry fee is charged once per participant, against their single largest-value row -
                // a GroupStage hero with both a GroupWinner and a playoff-tier row otherwise pays twice.
                var points = i == 0 ? rows[i].Points - entry : rows[i].Points;
                awards.Add(new TournamentAwardEntity
                {
                    TournamentId = tournamentId,
                    HeroId = heroId,
                    AwardKind = rows[i].Kind,
                    Points = points,
                    AwardedAt = awardedAt
                });
            }
        }

        // entry = pool/N truncates, so N*entry undercharges the field by `remainder` (0..N-1) - without
        // this, that many points would stay in circulation and the tournament wouldn't net to zero.
        if (remainder != 0 && awards.Count > 0)
        {
            awards.OrderByDescending(a => a.Points).First().Points -= remainder;
        }

        return awards;
    }

    /// <summary>SingleElimination's bracket, and reused for the playoff phase of GroupStage once its
    /// groups have advanced heroes into single elimination.</summary>
    private static (Dictionary<Guid, List<(TournamentAwardKind, double)>> Gross, Dictionary<Guid, int?> Placements)
        BracketSchedule(IReadOnlyList<MatchEntity> matches)
    {
        var bracketMatches = matches.Where(m => m.Stage.HasValue && m.Stage != Stage.Group).ToList();

        var eliminationStage = new Dictionary<Guid, Stage>();
        Guid? thirdPlaceWinner = null;
        Guid? thirdPlaceLoser = null;

        foreach (var match in bracketMatches.Where(m => m.Stage != Stage.GrandFinals))
        {
            var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);
            var loser = match.Fighters.FirstOrDefault(f => !f.IsWinner);
            if (winner is null || loser is null)
            {
                continue;
            }

            if (match.Stage == Stage.ThirdPlaceDecider)
            {
                // no extra points here - both finalists-for-third already earned the Semifinalist tier
                // from losing their semifinal. This match only decides the 3rd/4th placement label.
                thirdPlaceWinner = winner.HeroId;
                thirdPlaceLoser = loser.HeroId;
                continue;
            }

            eliminationStage[loser.HeroId] = match.Stage!.Value;
        }

        // the grand final is generated as three identical best-of-three stand-in matches (see
        // SingleEliminationGenerator) - the champion is whoever won the majority of them.
        var grandFinalsWins = new Dictionary<Guid, int>();
        foreach (var match in bracketMatches.Where(m => m.Stage == Stage.GrandFinals))
        {
            var winner = match.Fighters.FirstOrDefault(f => f.IsWinner);
            if (winner is not null)
            {
                grandFinalsWins[winner.HeroId] = grandFinalsWins.GetValueOrDefault(winner.HeroId) + 1;
            }
        }

        if (grandFinalsWins.Count == 0)
        {
            throw new InvalidOperationException("Cannot complete this tournament: it has not produced a champion yet.");
        }

        var championId = grandFinalsWins.OrderByDescending(kv => kv.Value).First().Key;
        var finalistId = bracketMatches
            .Where(m => m.Stage == Stage.GrandFinals)
            .SelectMany(m => m.Fighters)
            .Select(f => (Guid?)f.HeroId)
            .FirstOrDefault(id => id != championId);

        var gross = new Dictionary<Guid, List<(TournamentAwardKind, double)>>
        {
            [championId] = [(TournamentAwardKind.Winner, RatingConstants.WinnerAwardMultiplier)]
        };
        if (finalistId.HasValue)
        {
            gross[finalistId.Value] = [(TournamentAwardKind.Finalist, RatingConstants.FinalistAwardMultiplier)];
        }

        var placements = new Dictionary<Guid, int?> { [championId] = 1 };
        if (finalistId.HasValue)
        {
            placements[finalistId.Value] = 2;
        }

        if (thirdPlaceWinner.HasValue)
        {
            placements[thirdPlaceWinner.Value] = 3;
        }

        if (thirdPlaceLoser.HasValue)
        {
            placements[thirdPlaceLoser.Value] = 4;
        }

        foreach (var (heroId, stage) in eliminationStage)
        {
            var (kind, multiplier) = EliminationTier(stage);
            gross[heroId] = [(kind, multiplier)];
            placements.TryAdd(heroId, PlacementForEliminationStage(stage));
        }

        return (gross, placements);
    }

    private static (TournamentAwardKind Kind, double Multiplier) EliminationTier(Stage stage) => stage switch
    {
        Stage.SemiFinals => (TournamentAwardKind.Semifinalist, RatingConstants.SemifinalistAwardMultiplier),
        Stage.QuarterFinals => (TournamentAwardKind.Quarterfinalist, RatingConstants.QuarterfinalistAwardMultiplier),
        Stage.EighthFinals => (TournamentAwardKind.EighthFinalist, RatingConstants.EighthFinalistAwardMultiplier),
        Stage.SixteenthFinals => (TournamentAwardKind.SixteenthFinalist, RatingConstants.SixteenthFinalistAwardMultiplier),
        _ => (TournamentAwardKind.Eliminated, 0.0)
    };

    /// <summary>Standard bracket seeding placement: both semifinal losers tie for 3rd when no third-place
    /// match was recorded, and every earlier round ties at (2^depth + 1) since the bracket doesn't
    /// distinguish placement any further among same-round losers.</summary>
    private static int? PlacementForEliminationStage(Stage stage) => stage switch
    {
        Stage.SemiFinals => 3,
        Stage.QuarterFinals => 5,
        Stage.EighthFinals => 9,
        Stage.SixteenthFinals => 17,
        _ => null
    };

    private static (Dictionary<Guid, List<(TournamentAwardKind, double)>> Gross, Dictionary<Guid, int?> Placements)
        GroupStageSchedule(IReadOnlyList<MatchEntity> matches)
    {
        var groupMatches = matches.Where(m => m.Stage == Stage.Group).ToList();
        var playoffMatches = matches.Where(m => m.Stage.HasValue && m.Stage != Stage.Group).ToList();

        var (gross, placements) = BracketSchedule(playoffMatches);

        foreach (var group in GroupStageGenerator.DeriveGroups(groupMatches))
        {
            var groupWinnerId = SoleGroupWinner(group, groupMatches);
            if (groupWinnerId is null)
            {
                continue;
            }

            if (!gross.TryGetValue(groupWinnerId.Value, out var rows))
            {
                gross[groupWinnerId.Value] = rows = [];
            }

            rows.Add((TournamentAwardKind.GroupWinner, RatingConstants.GroupWinnerAwardMultiplier));
        }

        return (gross, placements);
    }

    private static Guid? SoleGroupWinner(List<Guid> group, List<MatchEntity> groupMatches)
    {
        var wins = group.ToDictionary(id => id, _ => 0);
        foreach (var fighter in groupMatches.Where(m => !m.IsPlanned).SelectMany(m => m.Fighters).Where(f => f.IsWinner))
        {
            if (wins.ContainsKey(fighter.HeroId))
            {
                wins[fighter.HeroId]++;
            }
        }

        var maxWins = wins.Values.DefaultIfEmpty(0).Max();
        if (maxWins == 0)
        {
            return null;
        }

        var topScorers = wins.Where(kv => kv.Value == maxWins).Select(kv => kv.Key).ToList();
        // a tied group forfeits the flat bonus rather than guessing who "really" won it.
        return topScorers.Count == 1 ? topScorers[0] : null;
    }

    private static (Dictionary<Guid, List<(TournamentAwardKind, double)>> Gross, Dictionary<Guid, int?> Placements)
        SwissSchedule(IReadOnlyList<TournamentParticipantEntity> participants, IReadOnlyList<MatchEntity> matches)
    {
        var ranked = SwissStandings.Compute(participants.Select(p => p.HeroId), matches)
            .OrderByDescending(s => s.Score)
            .ThenByDescending(s => s.Buchholz)
            .ThenBy(s => s.HeroId)
            .Select(s => s.HeroId)
            .ToList();

        var topHalfCount = (int)Math.Ceiling(ranked.Count / 2.0);

        var gross = new Dictionary<Guid, List<(TournamentAwardKind, double)>>();
        var placements = new Dictionary<Guid, int?>();
        for (var i = 0; i < ranked.Count; i++)
        {
            placements[ranked[i]] = i + 1;

            (TournamentAwardKind, double)? tier = i switch
            {
                0 => (TournamentAwardKind.Winner, RatingConstants.WinnerAwardMultiplier),
                1 => (TournamentAwardKind.Finalist, RatingConstants.FinalistAwardMultiplier),
                2 => (TournamentAwardKind.Semifinalist, RatingConstants.SemifinalistAwardMultiplier),
                _ when i < topHalfCount => (TournamentAwardKind.Quarterfinalist, RatingConstants.QuarterfinalistAwardMultiplier),
                _ => null
            };

            if (tier.HasValue)
            {
                gross[ranked[i]] = [tier.Value];
            }
        }

        return (gross, placements);
    }

    private static (Dictionary<Guid, List<(TournamentAwardKind, double)>> Gross, Dictionary<Guid, int?> Placements)
        LeagueSchedule(IReadOnlyList<TournamentParticipantEntity> participants, IReadOnlyList<MatchEntity> matches)
    {
        var wins = participants.ToDictionary(p => p.HeroId, _ => 0);
        var losses = participants.ToDictionary(p => p.HeroId, _ => 0);
        foreach (var fighter in matches.Where(m => !m.IsPlanned).SelectMany(m => m.Fighters))
        {
            if (!wins.ContainsKey(fighter.HeroId))
            {
                continue;
            }

            if (fighter.IsWinner)
            {
                wins[fighter.HeroId]++;
            }
            else
            {
                losses[fighter.HeroId]++;
            }
        }

        var ranked = participants
            .Select(p => p.HeroId)
            .OrderByDescending(id => wins[id])
            .ThenBy(id => losses[id])
            .ThenBy(id => id)
            .ToList();

        if (ranked.Count == 0 || wins[ranked[0]] == 0)
        {
            throw new InvalidOperationException("Cannot complete this tournament: it has not produced a champion yet.");
        }

        (TournamentAwardKind Kind, double Multiplier)[] tiers =
        [
            (TournamentAwardKind.SeasonFirst, RatingConstants.LeagueFirstAwardMultiplier),
            (TournamentAwardKind.SeasonSecond, RatingConstants.LeagueSecondAwardMultiplier),
            (TournamentAwardKind.SeasonThird, RatingConstants.LeagueThirdAwardMultiplier)
        ];

        var gross = new Dictionary<Guid, List<(TournamentAwardKind, double)>>();
        var placements = new Dictionary<Guid, int?>();
        for (var i = 0; i < ranked.Count; i++)
        {
            placements[ranked[i]] = i + 1;
            if (i < tiers.Length)
            {
                gross[ranked[i]] = [(tiers[i].Kind, tiers[i].Multiplier)];
            }
        }

        return (gross, placements);
    }
}
