namespace Unmatched.Dtos;

public class MatchDto
{
    public Guid Id { get; set; }

    public Guid? TournamentId { get; set; }

    public virtual TournamentDto? Tournament { get; set; }

    public Guid MapId { get; set; }

    public virtual MapDto Map { get; set; }

    public DateTime Date { get; set; }

    public string Comment { get; set; }
}