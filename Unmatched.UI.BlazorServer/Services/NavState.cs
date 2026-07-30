namespace Unmatched.UI.BlazorServer.Services;

/// <summary>
/// Whether the nav rail is expanded. Durable per browser (design/IMPLEMENTATION-PROMPT.md §6),
/// backed by <see cref="AppSettingsService"/> the same way the per-screen List/Tiles choice is -
/// the in-memory field keeps every page in a circuit in sync instantly, and the localStorage
/// round trip makes the choice survive a reload.
/// </summary>
public class NavState(AppSettingsService appSettingsService)
{
    private bool _isExpanded = true;
    private bool _loaded;

    public event Action? Changed;

    public bool IsExpanded => _isExpanded;

    /// <summary>
    /// Reads the persisted value via JS interop, which cannot run during static prerendering -
    /// callers load this from OnAfterRenderAsync(firstRender), not OnInitialized(Async).
    /// </summary>
    public async Task EnsureLoadedAsync()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;
        _isExpanded = await appSettingsService.GetNavExpandedAsync();
        Changed?.Invoke();
    }

    public async Task ToggleAsync()
    {
        _isExpanded = !_isExpanded;
        Changed?.Invoke();
        await appSettingsService.SetNavExpandedAsync(_isExpanded);
    }
}
