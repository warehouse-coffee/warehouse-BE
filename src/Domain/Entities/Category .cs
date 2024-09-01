namespace warehouse_BE.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string? Name { get; set; }

    // Relationships
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

