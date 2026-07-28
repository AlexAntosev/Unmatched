namespace Unmatched.UI.BlazorServer.Services;

using Unmatched.Services.Contracts;

/// <summary>
/// Feeds the nav rail's per-item counts. The numbers live in four different microservices, so
/// they are fetched once per circuit, in parallel, and cached until something that changes them
/// (saving a match, saving the collection) calls <see cref="Invalidate"/>.
/// </summary>
public class NavCountsService(
    IHeroService heroService,
    IPlayerService playerService,
    IMapService mapService,
    IMinionService minionService,
    IVillainService villainService,
    IMatchService matchService,
    IExpansionService expansionService,
    ICollectionService collectionService,
    ITitleService titleService,
    ITournamentService tournamentService)
{
    private NavCounts? _cached;

    public event Action? Changed;

    public async Task<NavCounts> GetAsync()
    {
        if (_cached is not null)
        {
            return _cached;
        }

        var heroes = heroService.GetAsync();
        var players = playerService.GetAsync();
        var maps = mapService.GetAsync();
        var minions = minionService.GetAsync();
        var villains = villainService.GetAsync();
        var matches = matchService.GetMatchLogAsync();
        var expansions = expansionService.GetAsync();
        var owned = collectionService.GetOwnedExpansionIdsAsync();
        var titles = titleService.GetAsync();
        var tournaments = tournamentService.GetAsync();

        await Task.WhenAll(heroes, players, maps, minions, villains, matches, expansions, owned, titles, tournaments);

        var ownedExpansionIds = owned.Result.ToHashSet();

        _cached = new NavCounts(
            CountOwned(heroes.Result, h => h.ExpansionId, ownedExpansionIds),
            players.Result.Count(),
            CountOwned(maps.Result, m => m.ExpansionId, ownedExpansionIds),
            CountOwned(minions.Result, m => m.ExpansionId, ownedExpansionIds),
            CountOwned(villains.Result, v => v.ExpansionId, ownedExpansionIds),
            matches.Result.Count(),
            ownedExpansionIds.Count,
            expansions.Result.Count(),
            titles.Result.Count(),
            tournaments.Result.Count());

        return _cached;
    }

    /// <summary>Entities from sets you do not own are not counted - unless nothing is marked as owned yet.</summary>
    private static int CountOwned<T>(IEnumerable<T> items, Func<T, Guid?> expansionId, HashSet<Guid> ownedExpansionIds)
    {
        if (ownedExpansionIds.Count == 0)
        {
            return items.Count();
        }

        return items.Count(item => expansionId(item) is null || ownedExpansionIds.Contains(expansionId(item)!.Value));
    }

    /// <summary>Drops the cache and asks the rail to re-read the counts.</summary>
    public void Invalidate()
    {
        _cached = null;
        Changed?.Invoke();
    }
}
