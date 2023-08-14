namespace Unmatched.Dtos;

public class SidekickDto
{
    public virtual HeroDto Hero { get; set; }

    public Guid HeroId { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public int Count { get; set; }
}
