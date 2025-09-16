using System.ComponentModel.DataAnnotations.Schema;

namespace Unmatched.MatchService.Domain.Entities;

[Table("HeroTitle")]
public class HeroTitleEntity
{
    public Guid TitlesId { get; set; }

    public Guid HeroesId { get; set; }
}
