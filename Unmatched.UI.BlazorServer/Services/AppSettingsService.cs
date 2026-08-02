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

    /// <summary>Nav rail expanded/collapsed - a durable preference per §6, not just circuit-scoped.</summary>
    private const string NavExpandedKey = "unmatched.navExpanded";

    /// <summary>The hero page's "Titles &amp; achievements" section, collapsed/expanded as one unit.</summary>
    private const string HeroTitlesSectionExpandedKey = "unmatched.heroTitlesSectionExpanded";

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

    public async Task<bool> GetNavExpandedAsync()
    {
        var module = await ModuleAsync();
        var raw = await module.InvokeAsync<string?>("getItem", NavExpandedKey);
        return bool.TryParse(raw, out var expanded) ? expanded : true;
    }

    public async Task SetNavExpandedAsync(bool expanded)
    {
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("setItem", NavExpandedKey, expanded.ToString());
    }

    public async Task<bool> GetHeroTitlesSectionExpandedAsync()
    {
        var module = await ModuleAsync();
        var raw = await module.InvokeAsync<string?>("getItem", HeroTitlesSectionExpandedKey);
        return bool.TryParse(raw, out var expanded) ? expanded : true;
    }

    public async Task SetHeroTitlesSectionExpandedAsync(bool expanded)
    {
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("setItem", HeroTitlesSectionExpandedKey, expanded.ToString());
    }

    private async Task<IJSObjectReference> ModuleAsync()
        => _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/appSettings.js");
}
