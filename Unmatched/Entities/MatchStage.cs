namespace Unmatched.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Unmatched.Enums;

public class MatchStage
{
    [Key]
    public Guid Id { get; set; }
    
    public virtual Match? Match { get; set; }

    [ForeignKey(nameof(Match))]
    public Guid? MatchId { get; set; }
    
    public Stage Stage { get; set; }
}
