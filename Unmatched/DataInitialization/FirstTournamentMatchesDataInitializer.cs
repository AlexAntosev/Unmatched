namespace Unmatched.DataInitialization;

using Unmatched.Constants;
using Unmatched.Entities;
using Unmatched.Repositories;

public class FirstTournamentMatchInfo 
{
    public Guid MapId { get; }

    public Guid AndriiHeroId { get; }

    public int AndriiHp { get; }

    public Guid OlexHeroId { get; }

    public int OlexHp { get; }

    public MatchLevel MatchLevel { get; }

    public FirstTournamentMatchInfo(string date, Guid mapId, Guid andriiHeroId, int andriiHp, Guid olexHeroId, int olexHp, MatchLevel matchLevel)
    {
        MapId = mapId;
        AndriiHeroId = andriiHeroId;
        AndriiHp = andriiHp;
        OlexHeroId = olexHeroId;
        OlexHp = olexHp;
        MatchLevel = matchLevel;
        Date = DateTime.Parse(date);
    }
    
    public DateTime Date { get; }
}

internal enum MatchLevel
{
    Group,
    QuarterFinals,
    SemiFinals,
    ThirdPlaceFinals,
    Finals
}

class FirstTournamentMatchesDataInitializer : IFirstTournamentMatchesDataInitializer
{
    private readonly IHeroRepository _heroRepository;

    private readonly IMapRepository _mapRepository;

    public FirstTournamentMatchesDataInitializer(IHeroRepository heroRepository, IMapRepository mapRepository)
    {
        _heroRepository = heroRepository;
        _mapRepository = mapRepository;
    }

    public IEnumerable<Match> GetEntities()
    {
        throw new NotImplementedException();
    }

    public async Task InitializeAsync()
    {
        var matches = new List<FirstTournamentMatchInfo>()
            {
                new("11/11/2022", GetMap(MapNames.GreenForest), GetHero(HeroNames.InvisibleMan), 7, GetHero(HeroNames.RobinHood), 0, MatchLevel.Group),
            }.OrderBy(x => x.Date).ToArray();

        foreach (var match in matches)
        {
            
        }

    }

    private Guid GetHero(string name)
    {
        return _heroRepository.GetIdByName(name);
    }

    private Guid GetMap(string name)
    {
        return _mapRepository.GetIdByName(name);
    }
}
