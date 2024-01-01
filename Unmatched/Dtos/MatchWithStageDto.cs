namespace Unmatched.Dtos;

using System;

using Unmatched.Entities;

public class MatchWithStageDto : MatchDto
{
    public Stage Stage { get; set; }
}
