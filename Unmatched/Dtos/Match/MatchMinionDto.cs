namespace Unmatched.Dtos.Match;

public class MatchMinionDto
{
    public Guid Id { get; set; }

    public Guid MatchVillainId { get; set; }

    public Guid MinionId { get; set; }

    public string? Name { get; set; }

    public int? HpLeft { get; set; }

    public bool IsWinner { get; set; }
}
