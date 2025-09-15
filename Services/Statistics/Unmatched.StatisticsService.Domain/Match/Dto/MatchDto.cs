namespace Unmatched.StatisticsService.Domain.Match.Dto;

using Unmatched.StatisticsService.Domain.Enums;

public class MatchDto
{
    public string? Comment { get; set; }

    public DateTime Date { get; set; }

    public int? Epic { get; set; }

    public IEnumerable<FighterDto> Fighters { get; set; }

    public Guid Id { get; set; }

    public bool IsPlanned { get; set; }

    public MapDto? Map { get; set; }

    public Stage? Stage { get; set; }

    public Guid? TournamentId { get; set; }
}
