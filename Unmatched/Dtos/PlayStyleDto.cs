namespace Unmatched.Dtos;

public class PlayStyleDto
{
    public int Attack { get; set; }

    public int Defence { get; set; }

    public int Difficulty { get; set; }

    public Guid HeroId { get; set; }

    public Guid Id { get; set; }

    public int Trickery { get; set; }

    public static PlayStyleDto Default(Guid heroId)
    {
        return new PlayStyleDto()
            {
                HeroId = heroId,
                Id = Guid.NewGuid()
            };
    }
}
