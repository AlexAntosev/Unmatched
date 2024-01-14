namespace Unmatched.Dtos;

public class PlayerDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public string ImageUrl => $"/{Name}.png";
}
