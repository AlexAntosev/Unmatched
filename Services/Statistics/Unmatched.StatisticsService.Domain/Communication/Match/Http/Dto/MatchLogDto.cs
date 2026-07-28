namespace Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;

using Unmatched.StatisticsService.Domain.Enums;

public class MatchLogDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public IEnumerable<FighterDto> Fighters { get; set; }

    public string MapName { get; set; }

    public Guid MatchId { get; set; }

    public string TournamentName { get; set; }

    public int? Epic { get; set; }

    public GameMode GameMode { get; set; }

    public MatchVillainDto? Villain { get; set; }
}
