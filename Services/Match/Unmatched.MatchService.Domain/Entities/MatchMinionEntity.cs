namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("MatchMinions")]
public class MatchMinionEntity
{
    [Key]
    public Guid Id { get; set; }

    public Guid MatchVillainId { get; set; }

    public Guid MinionId { get; set; }

    public int? HpLeft { get; set; }

    public bool IsWinner { get; set; }
}
