namespace warehouse_BE.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public string? Type { get; set; }

    public DateTime Date { get; set; } 

    public decimal TotalPrice { get; set; } 

    // Relationships
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
