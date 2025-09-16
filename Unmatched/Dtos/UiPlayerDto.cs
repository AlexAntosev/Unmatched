namespace Unmatched.Dtos;

public class UiPlayerDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }
    
    public string ImageUrl => $"/{Name}.png";
}
