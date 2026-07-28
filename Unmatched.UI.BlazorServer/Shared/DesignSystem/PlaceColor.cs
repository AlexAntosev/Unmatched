namespace Unmatched.UI.BlazorServer.Shared.DesignSystem;

/// <summary>
/// Gold / silver / bronze for the top three finishers, muted for everyone below. Used by ladder
/// rank numbers, free-for-all place badges and the Add match place picker.
/// </summary>
public static class PlaceColor
{
    public static string For(int? place)
        => place switch
        {
            1 => "var(--um-gold)",
            2 => "var(--um-silver)",
            3 => "var(--um-bronze)",
            _ => "var(--um-text-muted)"
        };
}
