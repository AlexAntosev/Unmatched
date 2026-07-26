namespace Unmatched.MatchService.Domain.Models;

public class MatchMinion
{
    public Guid Id { get; set; }

    public Guid MatchVillainId { get; set; }

    public Guid MinionId { get; set; }

    public string? Name { get; set; }

    public int? HpLeft { get; set; }

    public bool IsWinner { get; set; }
}
