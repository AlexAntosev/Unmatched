namespace Unmatched.MatchService.Domain.Models;

public class Rating
{
    public Guid HeroId { get; set; }

    public int Points { get; set; }
}