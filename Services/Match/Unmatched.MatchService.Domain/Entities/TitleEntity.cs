namespace Unmatched.MatchService.Domain.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Titles")]
public class TitleEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    [ForeignKey(nameof(HeroTitleEntity.TitlesId))]
    public virtual ICollection<HeroTitleEntity> HeroTitles { get; set; }
    
    public string Comment { get; set; }
}