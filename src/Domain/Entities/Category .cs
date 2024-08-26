namespace warehouse_BE.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty; // NOT NULL

    // Relationships
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

