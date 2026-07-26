namespace Unmatched.Dtos;

public class UiPlayerDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? ImageFileName { get; set; }

    public string ImageUrl => ImageFileName != null ? $"/images/players/{ImageFileName}" : "/Unknown.png";
}
