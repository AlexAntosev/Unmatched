namespace Unmatched.MatchService.Domain.Tournaments;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Enums;
using Unmatched.MatchService.Domain.Extensions;

/// <summary>
/// Splits the field into groups, round-robins every match within a group in one shot (no round
/// dependency - unlike a bracket, none of a group's matches gate any other), then once every group
/// match has finished, seeds a single-elimination bracket with the top <see cref="AdvanceCount"/> of
/// each group and hands the rest of the tournament to <see cref="SingleEliminationGenerator"/>.
/// </summary>
public class GroupStageGenerator(SingleEliminationGenerator singleEliminationGenerator) : ITournamentFormatGenerator
{
    private const int GroupSize = 4;
    private const int AdvanceCount = 2;

    public GroupStageGenerator() : this(new SingleEliminationGenerator())
    {
    }

    public bool CanGenerateNext(TournamentEntity tournament, IReadOnlyList<MatchEntity> existingMatches)
    {
        var groupMatches = existingMatches.Where(m => m.Stage == Stage.Group).ToList();
        if (groupMatches.Count == 0)
        {
            return true;
        }

        var playoffMatches = existingMatches.Where(m => m.Stage != Stage.Group).ToList();
        var syntheticTournament = new TournamentEntity { CurrentStage = playoffMatches.Select(m => m.Stage!.Value).DefaultIfEmpty(Stage.Group).Max() };
        return singleEliminationGenerator.CanGenerateNext(syntheticTournament, playoffMatches);
    }

    public (IReadOnlyList<TournamentPairing> Pairings, Stage? Stage, int? Round) GenerateNext(
        TournamentEntity tournament,
        IReadOnlyList<TournamentParticipantEntity> participants,
        IReadOnlyList<MatchEntity> existingMatches)
    {
        var groupMatches = existingMatches.Where(m => m.Stage == Stage.Group).ToList();

        if (groupMatches.Count == 0)
        {
            var groups = SplitIntoGroups(participants.Select(p => p.HeroId).ToList());
            var pairings = groups.SelectMany(RoundRobinPairs).ToList();
            return (pairings, Stage.Group, null);
        }

        var playoffMatches = existingMatches.Where(m => m.Stage != Stage.Group).ToList();

        if (playoffMatches.Count == 0)
        {
            // groups just finished - seed the playoff bracket with each group's top finishers. Group
            // membership is derived from the matches actually generated, not re-shuffled from
            // participants - re-shuffling here would produce different groups than the ones the
            // round-robin was actually played in.
            var groups = DeriveGroups(groupMatches);
            var advancing = groups.SelectMany(group => TopN(group, groupMatches, AdvanceCount)).ToList();
            var playoffParticipants = advancing.Select(heroId => new TournamentParticipantEntity { HeroId = heroId }).ToList();
            var seedingTournament = new TournamentEntity { CurrentStage = FirstEliminationStageFor(advancing.Count) };

            return singleEliminationGenerator.GenerateNext(seedingTournament, playoffParticipants, []);
        }

        // playoffs are under way - defer entirely to single-elimination progression.
        var currentPlayoffStage = playoffMatches.Max(m => m.Stage!.Value);
        var playoffTournament = new TournamentEntity { CurrentStage = currentPlayoffStage };
        return singleEliminationGenerator.GenerateNext(playoffTournament, [], playoffMatches);
    }

    /// <summary>Reconstructs group membership from the pairings actually generated for the group stage:
    /// a round robin makes each group a clique in the "played against" graph, so its connected
    /// components are exactly the groups, regardless of how many of those matches have been played yet.</summary>
    public static List<List<Guid>> DeriveGroups(IEnumerable<MatchEntity> groupMatches)
    {
        var adjacency = new Dictionary<Guid, HashSet<Guid>>();
        foreach (var match in groupMatches)
        {
            var heroIds = match.Fighters.Select(f => f.HeroId).ToList();
            foreach (var heroId in heroIds)
            {
                if (!adjacency.TryGetValue(heroId, out var neighbours))
                {
                    adjacency[heroId] = neighbours = [];
                }

                foreach (var other in heroIds.Where(id => id != heroId))
                {
                    neighbours.Add(other);
                }
            }
        }

        var visited = new HashSet<Guid>();
        var groups = new List<List<Guid>>();
        foreach (var heroId in adjacency.Keys)
        {
            if (!visited.Add(heroId))
            {
                continue;
            }

            var group = new List<Guid> { heroId };
            var queue = new Queue<Guid>(adjacency[heroId]);
            while (queue.Count > 0)
            {
                var next = queue.Dequeue();
                if (!visited.Add(next))
                {
                    continue;
                }

                group.Add(next);
                foreach (var neighbour in adjacency[next])
                {
                    queue.Enqueue(neighbour);
                }
            }

            groups.Add(group);
        }

        return groups;
    }

    private static List<List<Guid>> SplitIntoGroups(List<Guid> heroIds)
    {
        var shuffled = heroIds.Shuffle();
        var groups = new List<List<Guid>>();
        for (var i = 0; i < shuffled.Count; i += GroupSize)
        {
            groups.Add(shuffled.Skip(i).Take(GroupSize).ToList());
        }

        return groups;
    }

    private static IEnumerable<TournamentPairing> RoundRobinPairs(List<Guid> group)
    {
        for (var i = 0; i < group.Count; i++)
        {
            for (var j = i + 1; j < group.Count; j++)
            {
                yield return new TournamentPairing(group[i], group[j]);
            }
        }
    }

    private static List<Guid> TopN(List<Guid> group, List<MatchEntity> groupMatches, int n)
    {
        var wins = group.ToDictionary(id => id, _ => 0);
        foreach (var fighter in groupMatches.Where(m => !m.IsPlanned).SelectMany(m => m.Fighters).Where(f => f.IsWinner))
        {
            if (wins.ContainsKey(fighter.HeroId))
            {
                wins[fighter.HeroId]++;
            }
        }

        return group.OrderByDescending(id => wins[id]).Take(n).ToList();
    }

    private static Stage FirstEliminationStageFor(int participantCount) => participantCount switch
    {
        <= 2 => Stage.GrandFinals,
        <= 4 => Stage.SemiFinals,
        <= 8 => Stage.QuarterFinals,
        <= 16 => Stage.EighthFinals,
        _ => Stage.SixteenthFinals
    };
}
