namespace Unmatched.UI.BlazorServer.Services;

using Microsoft.JSInterop;
using Unmatched.UI.BlazorServer.Shared.DesignSystem;

/// <summary>
/// Durable, per-browser preferences owned by the Settings screen (README "Menus and settings").
/// Backed by localStorage via JS interop - these are cosmetic client preferences, not domain data,
/// so no backend table or migration is warranted.
/// </summary>
public class AppSettingsService(IJSRuntime jsRuntime)
{
    private const string MatchesPerPageKey = "unmatched.matchesPerPage";
    private const int DefaultMatchesPerPage = 10;

    /// <summary>List/Tiles is a per-screen preference (design/IMPLEMENTATION-PROMPT.md §1), keyed by screen.</summary>
    private const string ListViewModeKeyPrefix = "unmatched.listViewMode.";

    private IJSObjectReference? _module;

    public async Task<int> GetMatchesPerPageAsync()
    {
        var module = await ModuleAsync();
        var raw = await module.InvokeAsync<string?>("getItem", MatchesPerPageKey);
        return int.TryParse(raw, out var value) ? value : DefaultMatchesPerPage;
    }

    public async Task SetMatchesPerPageAsync(int value)
    {
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("setItem", MatchesPerPageKey, value.ToString());
    }

    public async Task<ListViewMode> GetListViewModeAsync(string screenKey)
    {
        var module = await ModuleAsync();
        var raw = await module.InvokeAsync<string?>("getItem", ListViewModeKeyPrefix + screenKey);
        return Enum.TryParse<ListViewMode>(raw, out var mode) ? mode : ListViewMode.List;
    }

    public async Task SetListViewModeAsync(string screenKey, ListViewMode mode)
    {
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("setItem", ListViewModeKeyPrefix + screenKey, mode.ToString());
    }

    private async Task<IJSObjectReference> ModuleAsync()
        => _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/appSettings.js");
}
