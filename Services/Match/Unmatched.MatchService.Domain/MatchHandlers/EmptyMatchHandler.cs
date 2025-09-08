namespace Unmatched.MatchService.Domain.MatchHandlers;

using Microsoft.Extensions.Logging;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Repositories;

public class EmptyMatchHandler : BaseMatchHandler
{
    private readonly ILogger<EmptyMatchHandler> _logger;

    public EmptyMatchHandler(
        IUnitOfWork unitOfWork,
        ILoggerFactory loggerFactory) : base(unitOfWork)
    {
        _logger = loggerFactory.CreateLogger<EmptyMatchHandler>();
    }

    protected override Task InnerHandleAsync(Match match)
    {
        _logger.LogError("Match was not handled due to handler was not found.");
        
        return Task.CompletedTask;
    }

    protected override void InnerValidate(Match match)
    {
    }
}
