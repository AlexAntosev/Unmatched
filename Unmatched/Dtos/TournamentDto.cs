namespace Unmatched.Dtos;

using Unmatched.Data.Enums;

public class TournamentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentType Type { get; set; }

    public bool IsActive { get; set; } = true;
    
    public Stage InitialStage { get; set; }
    
    public Stage CurrentStage { get; set; }
    
    public List<MatchDto> Matches { get; set; }
}
