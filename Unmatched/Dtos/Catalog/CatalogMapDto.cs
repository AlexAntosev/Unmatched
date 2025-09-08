namespace Unmatched.Dtos.Catalog;

public class CatalogMapDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string ImageUrl => $"/{Name.Replace(" ", string.Empty)}.png";
}
