namespace Unmatched.DataInitialization;

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
