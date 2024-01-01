namespace Unmatched.Dtos;

using Unmatched.Entities;

public class TournamentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentType Type { get; set; }
    
    public Stage CurrentStage { get; set; }
    
    public List<MatchDto> Matches { get; set; }
}

public enum TournamentType
{
    NonRating,

    Rating
}
