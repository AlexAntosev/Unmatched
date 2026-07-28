namespace Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;

public class MatchVillainDto
{
    public Guid VillainId { get; set; }

    public string? Name { get; set; }

    public bool IsWinner { get; set; }

    public IEnumerable<MatchMinionDto> Minions { get; set; } = new List<MatchMinionDto>();
}
