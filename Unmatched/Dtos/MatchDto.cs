namespace Unmatched.Dtos;

using Unmatched.Data.Enums;

public class MatchDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public Guid Id { get; set; }

    public MapDto? Map { get; set; }

    public Guid MapId { get; set; }

    public TournamentDto? Tournament { get; set; }
    
    public IEnumerable<FighterDto> Fighters { get; set; }

    public Guid? TournamentId { get; set; }
    
    public bool IsPlanned { get; set; }
    
    public Stage? Stage { get; set; }
}
