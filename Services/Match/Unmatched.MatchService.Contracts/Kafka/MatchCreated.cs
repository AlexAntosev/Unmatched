namespace Unmatched.MatchService.Contracts.Kafka;

public class MatchCreated
{
    public DateTime Date { get; set; }

    public IEnumerable<Fighter> Fighters { get; set; }

    public Guid Id { get; set; }

    public bool IsPlanned { get; set; }

    public Guid MapId { get; set; }

    public Guid? TournamentId { get; set; }

    public class Fighter
    {
        public Guid HeroId { get; set; }

        public bool IsWinner { get; set; }

        public int? MatchPoints { get; set; }

        public Guid PlayerId { get; set; }

        public int? ResultRating { get; set; }
    }
}
