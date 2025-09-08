namespace Unmatched.MatchService.Domain.Entities;

public class PlayStyle
{
    public int Attack { get; set; }

    public int Defence { get; set; }

    public int Difficulty { get; set; }

    public Guid HeroId { get; set; }

    public int Trickery { get; set; }

    public static PlayStyle Default(Guid heroId)
    {
        return new PlayStyle
            {
                HeroId = heroId
            };
    }
}
