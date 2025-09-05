namespace Unmatched.CatalogService.Api.Dto;

using System;


public class HeroDto
{
    public int DeckSize { get; set; }

    public int Hp { get; set; }

    public Guid Id { get; set; } 

    public bool IsRanged { get; set; }

    public string Name { get; set; }

    //public virtual ICollection<Sidekick> Sidekicks { get; set; }

    public string Color { get; set; }

    //public virtual ICollection<Title> Titles { get; set; }

    //public virtual PlayStyle? PlayStyle { get; set; }
}
