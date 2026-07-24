namespace Unmatched.Dtos;

public class MapDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? ImageFileName { get; set; }

    public string ImageUrl => ImageFileName != null ? $"/images/maps/{ImageFileName}" : "/UnknownMap.png";
}