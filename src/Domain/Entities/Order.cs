namespace warehouse_BE.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public string Type { get; set; } = string.Empty; // NOT NULL

    public DateTime Date { get; set; } // NOT NULL

    public int Total { get; set; } // NOT NULL

    // Relationships
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
