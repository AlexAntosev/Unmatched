using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class MatchParticipant
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid MatchId { get; set; }
    
    public virtual Match Match { get; set; }
    
    public Guid PlayerId { get; set; }
    
    public virtual Player Player { get; set; }
    
    public Guid HeroId { get; set; }
    
    public virtual Hero Hero { get; set; }
    
    public int HpLeft { get; set; }
    
    public int CardsLeft { get; set; }
    
    public int SidekickHpLeft { get; set; }
    
    public int ItemsUsed { get; set; }
    
    public int Turn { get; set; }
    
    public bool IsWinner { get; set; }
}