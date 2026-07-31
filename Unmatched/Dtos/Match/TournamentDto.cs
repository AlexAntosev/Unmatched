namespace Unmatched.Dtos.Match;

using Unmatched.Enums;

public class TournamentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public TournamentFormat Format { get; set; }

    public TournamentStatus Status { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int MaxParticipants { get; set; }

    public string? ImageFileName { get; set; }

    public string? TrophyImageFileName { get; set; }

    public Stage InitialStage { get; set; }

    public Stage CurrentStage { get; set; }

    public IEnumerable<Guid> ParticipantHeroIds { get; set; } = [];

    public IEnumerable<TournamentTitleKind> TitleKinds { get; set; } = [];

    public string? ImageUrl => ImageFileName != null ? $"/images/tournaments/{ImageFileName}" : null;

    public string? TrophyImageUrl => TrophyImageFileName != null ? $"/images/trophies/{TrophyImageFileName}" : null;
}
