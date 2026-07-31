namespace Unmatched.MatchService.Domain.Tournaments;

/// <summary>One match a format generator wants scheduled. A participant left without an opponent for
/// a round (an odd Swiss field, say) simply produces no pairing for that round - there's no explicit
/// "bye" representation, since a bye isn't a match.</summary>
public readonly record struct TournamentPairing(Guid HeroId, Guid OpponentHeroId);
