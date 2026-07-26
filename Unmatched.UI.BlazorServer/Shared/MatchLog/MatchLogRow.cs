namespace Unmatched.UI.BlazorServer.Shared.MatchLog;

using Unmatched.Dtos;
using Unmatched.Enums;

/// <summary>
/// Strongly-typed grid row for <see cref="MatchLogGrid"/>, replacing the previous dynamic
/// ExpandoObject shape. Common columns (Date/Tournament/Map/Comment/Epic) bind directly;
/// the "Participants" column template reads <see cref="Source"/> and branches on
/// <see cref="GameMode"/> since 1v1/2v2/free-for-all/co-op each shape participants differently.
/// </summary>
public class MatchLogRow
{
    public Guid MatchId { get; set; }

    public DateTime Date { get; set; }

    public string? Tournament { get; set; }

    public string? Map { get; set; }

    public string? Comment { get; set; }

    public int? Epic { get; set; }

    public GameMode GameMode { get; set; }

    public required UiMatchLogDto Source { get; set; }
}
