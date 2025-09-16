namespace Unmatched.Dtos;

public class UiMatchLogDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<UiFighterDto> Fighters { get; set; }

    public string MapName { get; set; }

    public Guid MatchId { get; set; }

    public string TournamentName { get; set; }

    public int? Epic { get; set; }
}
