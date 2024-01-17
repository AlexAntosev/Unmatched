namespace Unmatched.Services.TitleHandlers;

using Unmatched.Data.Entities;
using Unmatched.Dtos;

public interface IPunisherTitleHandler
{
    Task<TitleDto?> HandleAsync(Match match);
}
