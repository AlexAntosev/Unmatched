namespace Unmatched.Dtos;

public class FighterDto
{
    public Guid Id { get; set; }

    public Guid MatchId { get; set; }

    public Guid PlayerId { get; set; }

    public string PlayerName { get; set; }

    public Guid HeroId { get; set; }

    public string HeroName { get; set; }

    public int HpLeft { get; set; }

    public int CardsLeft { get; set; }

    public int SidekickHpLeft { get; set; }

    public int ItemsUsed { get; set; }

    public int Turn { get; set; }

    public bool IsWinner { get; set; }
}