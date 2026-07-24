namespace Unmatched.PlayerService.Api.Dto;

public class FavoriteDto
{
    public Guid HeroId { get; set; }

    public bool IsChosenOne { get; set; }

    public int Favour { get; set; }
}
