namespace Unmatched.Dtos.Match;

public class TournamentStandingDto
{
    public Guid HeroId { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int? FinalPlacement { get; set; }
}
