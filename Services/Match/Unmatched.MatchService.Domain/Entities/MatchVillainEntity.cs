namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("MatchVillains")]
public class MatchVillainEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }

    public Guid VillainId { get; set; }

    public int? HpLeft { get; set; }

    public int? CardsLeft { get; set; }

    public bool IsWinner { get; set; }

    [ForeignKey(nameof(MatchMinionEntity.MatchVillainId))]
    public virtual ICollection<MatchMinionEntity> Minions { get; set; } = new List<MatchMinionEntity>();
}
