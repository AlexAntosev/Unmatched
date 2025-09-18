namespace Unmatched.StatisticsService.Domain.Initialize.Coordinators;

using Unmatched.StatisticsService.Domain.Communication.Match.Http.Dto;
using Unmatched.StatisticsService.Domain.Repositories;

public interface IStatsCoordinator
{
    IUnitOfWork UnitOfWork { get; set; }
    Task<bool> HasDataAsync();

    Task InitializeAsync(IReadOnlyCollection<MatchDto> matches);

    Task CheckAndInitializeAsync();
}