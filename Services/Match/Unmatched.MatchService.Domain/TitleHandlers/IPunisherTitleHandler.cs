namespace Unmatched.MatchService.Domain.TitleHandlers;

using Unmatched.MatchService.Domain.Dto;
using Unmatched.MatchService.Domain.Entities;

public interface IPunisherTitleHandler
{
    Task<TitleDto?> HandleAsync(MatchEntity match);
}
