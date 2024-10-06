namespace warehouse_BE.Domain.Entities;

public class OrderDetail : BaseAuditableEntity
{
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Note { get; set; }
    // Relationships
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
