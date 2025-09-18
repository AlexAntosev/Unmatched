namespace Unmatched.StatisticsService.Domain.Initialize;

using Unmatched.StatisticsService.Domain.Initialize.Coordinators;
using Unmatched.StatisticsService.Domain.Repositories;

public interface IStatsCoordinatorProvider
{
    IEnumerable<IStatsCoordinator> GetAll(IUnitOfWork unitOfWork);
}