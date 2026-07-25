namespace Unmatched.Dtos;

public class MapDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? OfficialName { get; set; }

    public string? ImageFileName { get; set; }

    public Guid? ExpansionId { get; set; }

    public string? ExpansionName { get; set; }

    public string ImageUrl => ImageFileName != null ? $"/images/maps/{ImageFileName}" : "/UnknownMap.png";

    public string DisplayName => string.IsNullOrEmpty(OfficialName) || OfficialName == Name ? Name : $"{OfficialName} ({Name})";
}