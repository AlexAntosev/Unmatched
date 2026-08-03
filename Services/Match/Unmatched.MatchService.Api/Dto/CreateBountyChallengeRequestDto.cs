namespace Unmatched.MatchService.Api.Dto;

public class CreateBountyChallengeRequestDto
{
    public Guid ChallengerHeroId { get; set; }

    public Guid ChampionPlayerId { get; set; }

    public Guid ChallengerPlayerId { get; set; }

    public Guid MapId { get; set; }
}
