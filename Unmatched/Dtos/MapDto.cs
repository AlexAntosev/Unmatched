namespace Unmatched.Dtos;

public class MapDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ImageUrl => $"/{Name.Replace(" ", string.Empty)}.png";
}