namespace Unmatched.MatchService.Api.Dto;

using System;

public class TitleHolderDto
{
    public Guid HeroId { get; set; }

    public DateTime? EarnedAt { get; set; }

    public int TimesEarned { get; set; }

    public double? Metric { get; set; }
}
