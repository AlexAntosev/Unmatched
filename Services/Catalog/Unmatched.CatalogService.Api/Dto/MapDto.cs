namespace Unmatched.CatalogService.Api.Dto;

public class MapDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid? ExpansionId { get; set; }

    public string? ExpansionName { get; set; }

    public string? ImageFileName { get; set; }
}
