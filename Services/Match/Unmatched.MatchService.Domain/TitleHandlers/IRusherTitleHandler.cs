namespace Unmatched.MatchService.Domain.TitleHandlers;

using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;

public interface IRusherTitleHandler
{
    Task<TitleDto?> HandleAsync(MatchEntity match);
}
