namespace Unmatched.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class Fighter
{
    public int? CardsLeft { get; set; }

    public virtual Hero Hero { get; set; }

    public Guid HeroId { get; set; }

    public int? HpLeft { get; set; }

    [Key]
    public Guid Id { get; set; }

    public bool IsWinner { get; set; }

    public int? ItemsUsed { get; set; }

    public virtual Match Match { get; set; }
    
    public Guid MatchId { get; set; }

    public int? MatchPoints { get; set; }

    public virtual Player Player { get; set; }

    public Guid PlayerId { get; set; }

    public int? SidekickHpLeft { get; set; }

    public int? Turn { get; set; }
    
    public int? ActionsMade { get; set; }
    
    public int? TimeSpentInSeconds { get; set; }
}
