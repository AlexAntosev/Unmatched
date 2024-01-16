namespace Unmatched.Services.TitleHandlers;

using System;

using Unmatched.Data.Entities;

public interface IRusherTitleHandler
{
    Task HandleAsync(Match match);
}
