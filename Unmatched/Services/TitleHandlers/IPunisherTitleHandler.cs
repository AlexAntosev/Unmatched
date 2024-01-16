namespace Unmatched.Services.TitleHandlers;

using Unmatched.Data.Entities;

public interface IPunisherTitleHandler
{
    Task HandleAsync(Match match);
}
