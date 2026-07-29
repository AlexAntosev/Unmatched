namespace Unmatched.UI.BlazorServer.Shared.DesignSystem;

/// <summary>
/// One choice in a <see cref="SegmentedControl{TValue}"/>, <see cref="Tabs{TValue}"/>,
/// <see cref="UmSelect{TValue}"/> or <see cref="UmMultiSelect{TValue}"/>. <paramref name="Color"/>
/// tints the icon only (game modes carry their own hue); the label and background always come
/// from the selected/unselected state. <paramref name="ArtUrl"/>/<paramref name="AvatarUrl"/> are
/// the optional row thumbnail a dropdown menu item can carry (hero token vs. round player avatar).
/// <paramref name="Count"/> is the optional right-aligned tally a filter row shows (e.g. "58").
/// </summary>
public record SelectOption<TValue>(
    TValue Value,
    string Label,
    string? Icon = null,
    string? Color = null,
    string? ArtUrl = null,
    string? AvatarUrl = null,
    int? Count = null);
