namespace Unmatched.Dtos;

using Unmatched.Enums;

public class UiMatchDto
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public Guid Id { get; set; }// = Guid.NewGuid();

    public MapDto? Map { get; set; }

    public IEnumerable<UiFighterDto> Fighters { get; set; }

    public Guid? TournamentId { get; set; }

    public bool IsPlanned { get; set; }

    public Stage? Stage { get; set; }

    public int? Epic { get; set; }
}
