namespace Unmatched.StatisticsService.Domain.Match;

public class FighterDto
{
    public DateTime DateTime { get; set; }

    public Guid HeroId { get; set; }

    public bool IsWinner { get; set; }

    public int MatchPoints { get; set; }
}
