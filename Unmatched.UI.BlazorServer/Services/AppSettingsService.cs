namespace Unmatched.UI.BlazorServer.Services;

using Microsoft.JSInterop;

/// <summary>
/// Durable, per-browser preferences owned by the Settings screen (README "Menus and settings").
/// Backed by localStorage via JS interop - these are cosmetic client preferences, not domain data,
/// so no backend table or migration is warranted.
/// </summary>
public class AppSettingsService(IJSRuntime jsRuntime)
{
    private const string MatchesPerPageKey = "unmatched.matchesPerPage";
    private const int DefaultMatchesPerPage = 10;

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

    private async Task<IJSObjectReference> ModuleAsync()
        => _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/appSettings.js");
}
