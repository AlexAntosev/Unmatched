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

        _cached = new NavCounts(
            heroes.Result.Count(),
            players.Result.Count(),
            maps.Result.Count(),
            minions.Result.Count(),
            villains.Result.Count(),
            matches.Result.Count(),
            owned.Result.Count(),
            expansions.Result.Count(),
            titles.Result.Count(),
            tournaments.Result.Count());

        return _cached;
    }

    /// <summary>Drops the cache and asks the rail to re-read the counts.</summary>
    public void Invalidate()
    {
        _cached = null;
        Changed?.Invoke();
    }
}
