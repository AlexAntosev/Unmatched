namespace Unmatched.UI.BlazorServer.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

/// <summary>
/// Positions a dropdown/multiselect panel with JS-computed viewport coordinates instead of CSS
/// position:absolute nested inside whatever ancestor happens to render it. The latter gets clipped by
/// any ancestor with overflow:hidden (e.g. Panel) - moving to position:fixed, anchored to the trigger's
/// on-screen bounds, escapes that regardless of how deeply the dropdown is nested.
/// </summary>
public class DropdownPositionService(IJSRuntime jsRuntime)
{
    private IJSObjectReference? _module;

    public async Task PositionAsync(ElementReference trigger, ElementReference panel)
    {
        var module = await ModuleAsync();
        await module.InvokeVoidAsync("position", trigger, panel);
    }

    private async Task<IJSObjectReference> ModuleAsync()
        => _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/dropdownPosition.js");
}
