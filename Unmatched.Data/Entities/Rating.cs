namespace Unmatched.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class Rating
{
    //public virtual Hero Hero { get; set; }

    public Guid HeroId { get; set; }

    [Key]
    public Guid Id { get; set; }

    public int Points { get; set; }
}
