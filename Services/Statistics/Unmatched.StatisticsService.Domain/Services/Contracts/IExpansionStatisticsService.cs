namespace Unmatched.StatisticsService.Domain.Services.Contracts;

using Unmatched.StatisticsService.Domain.Models;

public interface IExpansionStatisticsService
{
    Task<IEnumerable<ExpansionStats>> GetExpansionsStatisticsAsync();
}
