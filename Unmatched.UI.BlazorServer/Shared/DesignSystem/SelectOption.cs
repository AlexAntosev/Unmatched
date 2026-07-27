namespace Unmatched.UI.BlazorServer.Shared.DesignSystem;

/// <summary>
/// One choice in a <see cref="SegmentedControl{TValue}"/> or <see cref="Tabs{TValue}"/>.
/// <paramref name="Color"/> tints the icon only (game modes carry their own hue); the label and
/// background always come from the selected/unselected state.
/// </summary>
public record SelectOption<TValue>(TValue Value, string Label, string? Icon = null, string? Color = null);
