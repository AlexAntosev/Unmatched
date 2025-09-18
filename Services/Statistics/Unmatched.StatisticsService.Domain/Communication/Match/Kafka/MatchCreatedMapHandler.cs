namespace Unmatched.StatisticsService.Domain.Communication.Match.Kafka;

using Unmatched.StatisticsService.Domain.Repositories;

public class MatchCreatedMapHandler : IMatchCreatedHandler
{
    public async Task HandleAsync(IUnitOfWork unitOfWork, MatchCreated matchCreated)
    {
        var mapStats = await unitOfWork.MapStats.GetAsync(matchCreated.MapId);
        if (mapStats != null)
        {
            mapStats.TotalMatches++;
            mapStats.LastMatchIncludedAt = matchCreated.Date;
            mapStats.ModifiedAt = DateTime.UtcNow;
        }
        else
        {
            // TODO: create new entry
            throw new KeyNotFoundException($"There is no map stats entry found for hero: {matchCreated.MapId}");
        }
        await unitOfWork.MapStats.AddOrUpdateAsync(mapStats);
    }
}