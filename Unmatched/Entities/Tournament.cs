using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class Tournament
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public TournamentType Type { get; set; }
}

public enum TournamentType
{
    NonRating,
    Rating
}