namespace Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;

public class MatchMinionDto
{
    public Guid MinionId { get; set; }

    public string? Name { get; set; }

    public bool IsWinner { get; set; }
}
