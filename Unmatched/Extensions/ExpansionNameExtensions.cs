namespace Unmatched.Extensions;

/// <summary>
/// Catalog names are stored in full ("Unmatched: Cobble &amp; Fog") because that is what is printed
/// on the box. Dense lists show them under a hero name in a 1fr column, where the shared
/// "Unmatched" prefix is pure noise, so it is trimmed for display only.
/// </summary>
public static class ExpansionNameExtensions
{
    private static readonly string[] Prefixes =
    [
        "Unmatched Adventures: ",
        "Unmatched Marvel: ",
        "Unmatched: "
    ];

    public static string? ToShortName(this string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return fullName;
        }

        var prefix = Prefixes.FirstOrDefault(p => fullName.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        return prefix is null ? fullName : fullName[prefix.Length..];
    }
}
