namespace Unmatched.Dtos;

public class FighterDto
{
    public int? CardsLeft { get; set; }
    
    public HeroDto Hero { get; set; }

    public Guid HeroId { get; set; }

    public string HeroName { get; set; }

    public string HeroColor { get; set; }
    
    public int? MatchPoints { get; set; }

    public int? HpLeft { get; set; }

    public Guid Id { get; set; }

    public bool IsWinner { get; set; }

    public int? ItemsUsed { get; set; }

    public Guid MatchId { get; set; }

    public Guid PlayerId { get; set; }

    public string PlayerName { get; set; }
    
    public string? SidekickName { get; set; }

    public int? SidekickHpLeft { get; set; }

    public int? Turn { get; set; }
}
