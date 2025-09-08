namespace Unmatched.CatalogService.Contracts.Kafka.Events;

using System;


public class PlayStyleUpdated
{
    public Guid HeroId { get; set; }

    public int Attack { get; set; }

    public int Defence { get; set; }

    public int Trickery { get; set; }

    public int Difficulty { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
