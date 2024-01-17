namespace Unmatched.Services.TitleHandlers;

using Unmatched.Data.Entities;
using Unmatched.Dtos;

public interface IRusherTitleHandler
{
    Task<TitleDto?> HandleAsync(Match match);
}
