namespace Unmatched.DataInitialization.Matches;

using Unmatched.Data.Repositories;

public abstract class BaseMatchDataInitializer
{
    private readonly IHeroRepository _heroRepository;

    private readonly IMapRepository _mapRepository;

    private readonly IPlayerRepository _playerRepository;

    private readonly ITournamentRepository _tournamentRepository;

    protected BaseMatchDataInitializer(
        IHeroRepository heroRepository,
        IMapRepository mapRepository,
        IPlayerRepository playerRepository,
        ITournamentRepository tournamentRepository)
    {
        _heroRepository = heroRepository;
        _mapRepository = mapRepository;
        _playerRepository = playerRepository;
        _tournamentRepository = tournamentRepository;
    }

    protected Guid GetHero(string name)
    {
        return _heroRepository.GetIdByName(name);
    }

    protected Guid GetMap(string name)
    {
        return _mapRepository.GetIdByName(name);
    }

    protected Guid GetPlayer(string name)
    {
        return _playerRepository.GetIdByName(name);
    }
    
    protected Guid? GetTournament(string name)
    {
        return _tournamentRepository.GetIdByName(name);
    }
}
