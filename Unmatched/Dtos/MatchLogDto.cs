namespace Unmatched.Dtos;

public class MatchLogDto
{
    public DateTime Date { get; set; }
    
    public Guid MatchId { get; set; }
    public string Comment { get; set; }
    public string MapName { get; set; }
    public string TournamentName { get; set; }

    public IEnumerable<FighterDto> Fighters { get; set; }
}