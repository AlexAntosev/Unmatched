namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Ratings")]
public class RatingEntity
{
    public Guid HeroId { get; set; }

    [Key]
    public Guid Id { get; set; }

    public int Points { get; set; }
}
