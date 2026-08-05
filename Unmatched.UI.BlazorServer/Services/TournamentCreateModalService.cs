namespace Unmatched.UI.BlazorServer.Services;

/// <summary>
/// Lets any page ask for the "New tournament" modal without nesting a dialog component on every
/// page - TournamentCreateModal.razor is mounted once in MainLayout and subscribes to
/// <see cref="OpenRequested"/>, the same plain-event pattern as <see cref="NewMatchModalService"/>.
/// </summary>
public class TournamentCreateModalService
{
    public event Action? OpenRequested;

    public void RequestOpen() => OpenRequested?.Invoke();
}
