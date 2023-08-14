namespace Unmatched.Entities;

using System.ComponentModel.DataAnnotations;

public class Match
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    [Key]
    public Guid Id { get; set; }

    public virtual Map? Map { get; set; }

    public Guid? MapId { get; set; }

    public virtual Tournament? Tournament { get; set; }

    public Guid? TournamentId { get; set; }
}
