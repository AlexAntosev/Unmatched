using System.ComponentModel.DataAnnotations.Schema;

namespace Unmatched.MatchService.Domain.Entities;

[Table("HeroTitle")]
public class HeroTitleEntity
{
    public Guid TitlesId { get; set; }

    public Guid HeroesId { get; set; }

    /// <summary>Nullable because existing rows genuinely have no date - they were assigned manually
    /// before this column existed.</summary>
    public DateTime? EarnedAt { get; set; }
}
