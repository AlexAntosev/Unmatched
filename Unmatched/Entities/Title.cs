namespace Unmatched.Entities;

using System;
using System.ComponentModel.DataAnnotations;

public class Title
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public virtual ICollection<Hero> Heroes { get; set; }
    
    public string Comment { get; set; }
}
