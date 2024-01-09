namespace Unmatched.Data.Entities;

using System.ComponentModel.DataAnnotations;

public class Map
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; }
}
