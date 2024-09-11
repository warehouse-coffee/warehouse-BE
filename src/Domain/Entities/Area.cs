namespace warehouse_BE.Domain.Entities;

public class Area : BaseAuditableEntity
{
    public required string Name { get; set; } 

    // Relationships
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

