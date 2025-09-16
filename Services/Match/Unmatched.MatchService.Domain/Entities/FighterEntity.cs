namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Fighters")]
public class FighterEntity
{
    public int? ActionsMade { get; set; }

    public int? CardsLeft { get; set; }

    public Guid HeroId { get; set; }

    public int? HpLeft { get; set; }

    [Key]
    public Guid Id { get; set; }

    public bool IsWinner { get; set; }

    public int? ItemsUsed { get; set; }

    public virtual MatchEntity Match { get; set; }

    public Guid MatchId { get; set; }

    public int? MatchPoints { get; set; }

    public Guid PlayerId { get; set; }

    public int? SidekickHpLeft { get; set; }

    public int? TimeSpentInSeconds { get; set; }

    public int? Turn { get; set; }
}
