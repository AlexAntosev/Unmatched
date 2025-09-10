namespace Unmatched.MatchService.Domain.Dto;

public class MatchLog
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<Fighter> Fighters { get; set; }

    public string MapName { get; set; }

    public Guid MatchId { get; set; }

    public string TournamentName { get; set; }
    
    public int? Epic { get; set; }
}
