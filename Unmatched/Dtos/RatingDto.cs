namespace Unmatched.Dtos;

public class RatingDto
{
    public Guid Id { get; set; }
    
    public Guid TournamentId { get; set; }
    
    public virtual TournamentDto Tournament { get; set; }
    
    public Guid HeroId { get; set; }
    
    public virtual HeroDto Hero { get; set; }
    
    public int Points { get; set; }
}