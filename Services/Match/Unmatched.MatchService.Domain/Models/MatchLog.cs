namespace Unmatched.MatchService.Domain.Models;

using Unmatched.MatchService.Domain.Enums;

public class MatchLog
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<Fighter> Fighters { get; set; }

    public string MapName { get; set; }

    public Guid MatchId { get; set; }

    public string TournamentName { get; set; }

    public int? Epic { get; set; }

    public GameMode GameMode { get; set; }

    public MatchVillain? Villain { get; set; }
}
