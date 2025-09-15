namespace Unmatched.MatchService.Domain.Models;

using Unmatched.MatchService.Domain.Communication.Catalog.Dto;
using Unmatched.MatchService.Domain.Enums;

public class Match
{
    public string Comment { get; set; }

    public DateTime Date { get; set; }

    public Guid Id { get; set; }
    
    public CatalogMapDto? Map { get; set; }
    
    public IEnumerable<Fighter> Fighters { get; set; }

    public Guid? TournamentId { get; set; }
    
    public bool IsPlanned { get; set; }
    
    public Stage? Stage { get; set; }
    
    public int? Epic { get; set; }
}
