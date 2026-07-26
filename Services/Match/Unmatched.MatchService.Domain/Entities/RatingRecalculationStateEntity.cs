namespace Unmatched.MatchService.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RatingRecalculationState")]
public class RatingRecalculationStateEntity
{
    [Key]
    public Guid Id { get; set; }

    public bool IsRecalculationRequired { get; set; }
}
