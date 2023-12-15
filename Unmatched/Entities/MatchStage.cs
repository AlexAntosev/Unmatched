namespace Unmatched.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum Stage
{
    Group,
    QuarterFinals,
    SemiFinals,
    ThirdPlaceFinals,
    Finals
}

public class MatchStage
{
    [Key]
    public Guid Id { get; set; }
    
    public virtual Match? Match { get; set; }

    [ForeignKey(nameof(Match))]
    public Guid? MatchId { get; set; }
    
    public Stage Stage { get; set; }
}
