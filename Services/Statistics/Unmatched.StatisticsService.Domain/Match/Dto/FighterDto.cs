namespace Unmatched.StatisticsService.Domain.Match.Dto;

public class FighterDto
{
    public int? ActionsMade { get; set; }

    public int? CardsLeft { get; set; }

    public FighterHeroDto? Hero { get; set; }

    public Guid HeroId { get; set; }

    public int? HpLeft { get; set; }

    public Guid Id { get; set; }

    public bool IsWinner { get; set; }

    public int? ItemsUsed { get; set; }

    public Guid MatchId { get; set; }

    public int? MatchPoints { get; set; }

    public FighterPlayerDto? Player { get; set; }

    public int? SidekickHpLeft { get; set; }

    public int? TimeSpentInSeconds { get; set; }

    public int? Turn { get; set; }
}