namespace Unmatched.Services.MatchHandlers;

using Microsoft.Extensions.Logging;

using Unmatched.Entities;

public class EmptyMatchHandler : BaseMatchHandler
{
    private readonly ILogger<EmptyMatchHandler> _logger;

    public EmptyMatchHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<EmptyMatchHandler>();
    }

    protected override async Task InnerHandleAsync(Match match)
    {
        _logger.LogError("Match was not handled due to handler was not found.");
    }
}
