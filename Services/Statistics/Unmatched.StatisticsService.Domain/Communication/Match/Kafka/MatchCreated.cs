namespace Unmatched.StatisticsService.Domain.Communication.Match.Kafka;

public class MatchCreated
{
    public DateTime Date { get; set; }

    public IEnumerable<Fighter> Fighters { get; set; }

    public Guid Id { get; set; }

    public bool IsPlanned { get; set; }

    public Guid MapId { get; set; }

    public Guid? TournamentId { get; set; }

    public GameMode GameMode { get; set; }

    public MatchVillain? Villain { get; set; }

    public class Fighter
    {
        public Guid HeroId { get; set; }

        public bool IsWinner { get; set; }

        public int? MatchPoints { get; set; }

        public Guid PlayerId { get; set; }

        public int? ResultRating { get; set; }

        public int? Team { get; set; }

        public int? Placement { get; set; }
    }

    public class MatchVillain
    {
        public Guid VillainId { get; set; }

        public string? Name { get; set; }

        public bool IsWinner { get; set; }

        public IEnumerable<MatchMinion> Minions { get; set; } = new List<MatchMinion>();
    }

    public class MatchMinion
    {
        public Guid MinionId { get; set; }

        public string? Name { get; set; }

        public bool IsWinner { get; set; }
    }
}
