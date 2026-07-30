namespace Unmatched.UI.BlazorServer.Shared.DesignSystem;

/// <summary>
/// List ⇄ Tiles, the per-screen preference every ladder screen carries (design/IMPLEMENTATION-PROMPT.md §1).
/// </summary>
public enum ListViewMode
{
    List,
    Tiles
}

public static class ListViewModeOptions
{
    public static readonly IReadOnlyList<SelectOption<ListViewMode>> Default =
    [
        new(ListViewMode.List, string.Empty, "bi-list-ul"),
        new(ListViewMode.Tiles, string.Empty, "bi-grid-3x3-gap-fill")
    ];
}
