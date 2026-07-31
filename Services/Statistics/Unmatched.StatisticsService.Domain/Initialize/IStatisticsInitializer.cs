namespace Unmatched.StatisticsService.Domain.Initialize;

public interface IStatisticsInitializer
{
    Task InitializeAsync();

    /// <summary>Rebuilds every coordinator's statistics from scratch, regardless of whether it
    /// already has data - unlike <see cref="InitializeAsync"/>, which only fills in what's missing on
    /// startup. This is what makes a hero rating rebase (<c>RatingService.RecalculateAsync</c>) or a
    /// backfilled match actually show up in the read models: they never publish a Kafka event, so
    /// nothing else resyncs HeroStats/MapStats/VillainStats/MinionStats automatically.</summary>
    Task RebuildAsync();
}