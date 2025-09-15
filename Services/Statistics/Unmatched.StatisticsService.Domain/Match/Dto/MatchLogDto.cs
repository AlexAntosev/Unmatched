namespace Unmatched.StatisticsService.Domain.Match.Dto;

public class MatchLogDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<FighterDto> Fighters { get; set; }

    public string MapName { get; set; }

    public Guid MatchId { get; set; }

    public string TournamentName { get; set; }
    
    public int? Epic { get; set; }
}
