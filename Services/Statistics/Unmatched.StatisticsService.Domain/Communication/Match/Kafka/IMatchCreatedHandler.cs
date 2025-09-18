namespace Unmatched.StatisticsService.Domain.Communication.Match.Kafka;

using Unmatched.StatisticsService.Domain.Repositories;

public interface IMatchCreatedHandler
{
    Task HandleAsync(IUnitOfWork unitOfWork, MatchCreated matchCreated);
}
