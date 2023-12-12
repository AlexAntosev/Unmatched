namespace Unmatched.Entities;

using System;
using System.ComponentModel.DataAnnotations;

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

    public Guid? MatchId { get; set; }
    
    public Stage Stage { get; set; }
}
