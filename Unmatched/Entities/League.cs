using System.ComponentModel.DataAnnotations;

namespace Unmatched.Entities;

public class League
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
}