namespace Unmatched.Entities;

using System.ComponentModel.DataAnnotations;

public class Hero
{
    public Hero()
    {
        Id = Guid.NewGuid();
    }

    public int DeckSize { get; set; }

    public int Hp { get; set; }

    [Key]
    public Guid Id { get; set; }

    public bool IsRanged { get; set; }

    public string Name { get; set; }

    public IEnumerable<Sidekick> Sidekicks { get; set; }
}
