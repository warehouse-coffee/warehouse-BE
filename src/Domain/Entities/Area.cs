namespace warehouse_BE.Domain.Entities;

public class Area : BaseAuditableEntity
{
    public required string Name { get; set; }

    // Relationships
    public int StorageId { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

