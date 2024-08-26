namespace warehouse_BE.Domain.Entities;

public class Product : BaseAuditableEntity
{
    
    public string Name { get; set; } = string.Empty; // NOT NULL
    public int? CategoryId { get; set; } // Có thể null
    public int? AreaId { get; set; } // Có thể null
    public string Units { get; set; } = null!; // 'Kg, Pounds,...'
    public int Amount { get; set; }
    public string? Image { get; set; }
    public string Status { get; set; } = null!; // 'Sold or Not Sold,...'
    public DateTime Expiration { get; set; }
    public DateTime ImportDate { get; set; }
    public DateTime? ExportDate { get; set; }

    // Relationships
    public Category? Category { get; set; }
    public Area? Area { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

}


