namespace Unmatched.UI.BlazorServer.Services;

/// <summary>
/// Lets any page ask for the "New match" entry modal (Manual entry / Draft session) without
/// nesting a dialog component on every page - NewMatchModal.razor is mounted once in MainLayout
/// and subscribes to <see cref="OpenRequested"/>, the same plain-event pattern as
/// <see cref="NavState"/>/<see cref="NavCountsService"/>.
/// </summary>
public class NewMatchModalService
{
    public event Action? OpenRequested;

    public void RequestOpen() => OpenRequested?.Invoke();
}
