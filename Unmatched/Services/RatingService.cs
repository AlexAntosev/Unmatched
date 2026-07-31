using Unmatched.Services.Contracts;

namespace Unmatched.Services;

using Unmatched.HttpClients.Contracts;

public class RatingService(IMatchClient client, IStatisticsClient statisticsClient) : IRatingService
{
    /// <remarks>
    /// A hero rating recalculation never publishes the match-created Kafka event the statistics read
    /// models are normally kept in sync by, so on its own it would leave HeroStats/MapStats/etc.
    /// showing pre-recalculation points. Chaining the Statistics rebuild here makes that structurally
    /// impossible to forget - there is exactly one button, and it always does both.
    /// </remarks>
    public async Task RecalculateAsync()
    {
        await client.RecalculateAsync();
        await statisticsClient.RebuildAsync();
    }

    public Task<bool> IsRecalculationRequiredAsync()
    {
        return client.IsRatingRecalculationRequiredAsync();
    }
}
