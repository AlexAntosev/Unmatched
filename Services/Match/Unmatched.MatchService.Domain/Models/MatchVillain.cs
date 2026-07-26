namespace Unmatched.MatchService.Domain.Models;

public class MatchVillain
{
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }

    public Guid VillainId { get; set; }

    public string? Name { get; set; }

    public int? HpLeft { get; set; }

    public int? CardsLeft { get; set; }

    public bool IsWinner { get; set; }

    public IEnumerable<MatchMinion> Minions { get; set; } = new List<MatchMinion>();
}
