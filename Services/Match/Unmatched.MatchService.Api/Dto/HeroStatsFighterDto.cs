namespace Unmatched.MatchService.Api.Dto;

/// <summary>
///  temp dto for hero stats service. The plan is: stats service will request all matches with regular dtos to recalculates initial snapshot, and use kafka event to update it. This Dto will be removed
/// </summary>
public class HeroStatsFighterDto
{
    public DateTime DateTime { get; set; }

    public Guid HeroId { get; set; }

    public bool IsWinner { get; set; }

    public int MatchPoints { get; set; }
}
