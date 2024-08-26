namespace warehouse_BE.Domain.Entities;

public class Area : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty; // NOT NULL

    // Relationships
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Storage> Storages { get; set; } = new List<Storage>();
}

