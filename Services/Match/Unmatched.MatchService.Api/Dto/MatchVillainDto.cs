namespace Unmatched.MatchService.Api.Dto;

public class MatchVillainDto
{
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }

    public Guid VillainId { get; set; }

    public string? Name { get; set; }

    public int? HpLeft { get; set; }

    public int? CardsLeft { get; set; }

    public bool IsWinner { get; set; }

    public IEnumerable<MatchMinionDto> Minions { get; set; } = new List<MatchMinionDto>();
}
