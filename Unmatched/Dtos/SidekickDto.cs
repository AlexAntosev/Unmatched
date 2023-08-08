namespace Unmatched.Dtos;

public class SidekickDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public int Hp { get; set; }
    
    public Guid HeroId { get; set; }
    
    public virtual HeroDto Hero { get; set; }
}