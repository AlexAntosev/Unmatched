namespace Unmatched.Dtos.Catalog;

public class CatalogMapDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string? OfficialName { get; set; }

    public string? ImageFileName { get; set; }

    public Guid? ExpansionId { get; set; }

    public string? ExpansionName { get; set; }
}
