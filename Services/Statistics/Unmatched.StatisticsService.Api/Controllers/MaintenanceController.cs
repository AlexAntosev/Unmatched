namespace Unmatched.StatisticsService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Unmatched.StatisticsService.Domain.Initialize;

/// <summary>Operational actions that don't belong to any one statistics resource.</summary>
[ApiController]
[Route("[controller]")]
public class MaintenanceController(IStatisticsInitializer statisticsInitializer) : ControllerBase
{
    /// <summary>Rebuilds every read model (heroes/maps/villains/minions) from the match service's
    /// current data. Needed after a hero rating recalculation, since that replay never publishes the
    /// <c>match-created</c> Kafka event these read models are normally kept in sync by.</summary>
    [HttpPost("rebuild")]
    public async Task<IActionResult> Rebuild()
    {
        await statisticsInitializer.RebuildAsync();
        return NoContent();
    }
}
