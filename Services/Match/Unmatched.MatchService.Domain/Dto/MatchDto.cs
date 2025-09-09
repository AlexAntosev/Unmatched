namespace Unmatched.MatchService.Domain.Dto;

using Unmatched.MatchService.Domain.Enums;

public class MatchDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public Guid Id { get; set; }
    
    public Guid? MapId { get; set; }
    
    public IEnumerable<FighterDto> Fighters { get; set; }

    public Guid? TournamentId { get; set; }
    
    public bool IsPlanned { get; set; }
    
    public Stage? Stage { get; set; }
    
    public int? Epic { get; set; }
}
