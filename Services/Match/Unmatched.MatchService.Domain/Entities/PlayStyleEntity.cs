using System.ComponentModel.DataAnnotations.Schema;

namespace Unmatched.MatchService.Domain.Entities;

[Table("PlayStyle")]
public class PlayStyleEntity
{
    public int Attack { get; set; }

    public int Defence { get; set; }

    public int Difficulty { get; set; }

    public Guid HeroId { get; set; }

    public int Trickery { get; set; }

    public static PlayStyleEntity Default(Guid heroId)
    {
        return new PlayStyleEntity
            {
                HeroId = heroId
            };
    }
}
