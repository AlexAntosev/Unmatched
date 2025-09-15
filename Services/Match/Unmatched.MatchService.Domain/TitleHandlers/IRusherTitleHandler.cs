namespace Unmatched.MatchService.Domain.TitleHandlers;

using Unmatched.MatchService.Domain.Entities;
using Unmatched.MatchService.Domain.Models;

public interface IRusherTitleHandler
{
    Task<Title?> HandleAsync(MatchEntity match);
}
