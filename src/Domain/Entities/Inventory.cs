namespace warehouse_BE.Domain.Entities;

public class Inventory : BaseAuditableEntity
{
    public required string ProductName { get; set; } 

    public int TotalQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity => TotalQuantity - ReservedQuantity;

    public DateTime? Expiration { get; set; }

    public decimal TotalPrice { get; set; }
    public decimal TotalSalePrice { get; set; } 

    public int SafeStock { get; set; }

    public int CategoryId { get; set; }
    public int StorageId { get; set; }
    public Storage? Storage { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

