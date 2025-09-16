namespace Unmatched.CatalogService.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations;

public class Villain
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Name { get; set; }
    
    public int Hp { get; set; }

    public int DeckSize { get; set; }

    public bool IsRanged { get; set; }
    
    public string Color { get; set; }
}
