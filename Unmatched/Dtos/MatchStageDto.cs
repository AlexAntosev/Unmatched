namespace Unmatched.Dtos;

using System;

using Unmatched.Entities;

public class MatchStageDto
{
    public Guid Id { get; set; }
    
    public virtual MatchDto? Match { get; set; }

    public Guid? MatchId { get; set; }
    
    public Stage Stage { get; set; }
}
