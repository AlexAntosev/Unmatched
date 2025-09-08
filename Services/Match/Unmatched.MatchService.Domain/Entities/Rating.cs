namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Rating
{
    public Guid HeroId { get; set; }

    [Key]
    public Guid Id { get; set; }

    public int Points { get; set; }
}
