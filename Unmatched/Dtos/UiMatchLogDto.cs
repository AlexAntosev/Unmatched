namespace Unmatched.Dtos;

using Unmatched.Enums;

public class UiMatchLogDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<UiFighterDto> Fighters { get; set; }

    public string MapName { get; set; }

    public Guid MatchId { get; set; }

    public Guid? TournamentId { get; set; }

    public string TournamentName { get; set; }

    public bool IsRanked { get; set; }

    public int? Epic { get; set; }

    public GameMode GameMode { get; set; }

    public UiMatchVillainDto? Villain { get; set; }
}
