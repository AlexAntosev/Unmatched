namespace Unmatched.MatchService.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Title
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    [ForeignKey(nameof(HeroTitle.TitlesId))]
    public virtual ICollection<HeroTitle> HeroTitles { get; set; }
    
    public string Comment { get; set; }
}